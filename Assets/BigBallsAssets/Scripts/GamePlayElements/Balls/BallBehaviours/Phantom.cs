using System;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Phantom : EffectBehaviour
{
    private readonly PhantomConfig _config;

    private Ball _hostBall;
    private IEntityRepository _entityRepository;
    private IDamageService _damageService;
    private CurseRegistry _curseRegistry;

    public Phantom (PhantomConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (
        IEntityRepository entityRepository,
        IDamageService damageService,
        CurseRegistry curseRegistry)
    {
        _entityRepository = entityRepository;
        _damageService = damageService;
        _curseRegistry = curseRegistry;
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        if (host is not Ball ball)
            return new DisposableObject(() => { });

        _hostBall = ball;

        host.EventHandler.HitedEntity += OnHit;
        host.EventHandler.Killed += OnKill;

        return new DisposableObject(() =>
        {
            host.EventHandler.HitedEntity -= OnHit;
            host.EventHandler.Killed -= OnKill;
        });
    }

    private void OnKill (IEntity self, IEntity killed)
    {
        float perKill = _config.GetSpeedBonusPerKill(Level);
        float max = _config.GetMaxSpeedBonus(Level);

        StatHolder statHolder = _entityRepository.GetStatHolder(_hostBall);
        statHolder.TryGetStat(out Stat speed, StatType.MoveSpeed);
        speed.AddCurrentValue(Mathf.Min(perKill, max) * speed.CurrentValue);
    }

    private void OnHit (IEntity entity)
    {
        if (entity is not Enemy enemy)
            return;

        bool firstApplication = _curseRegistry.EnsureCurse(
            enemy,
            died =>
            {
                _curseRegistry.RemoveCurse(died);
                (died as Enemy)?.EffectViewer.DisableEffect(EffectType.Curce);
            });

        if (firstApplication)
            enemy.EffectViewer.EnableEffect(EffectType.Curce);

        int hits = _curseRegistry.AddHit(enemy);
        int needed = _config.GetHitsToProc(Level);

        if (hits < needed)
            return;

        StatHolder statHolder = _entityRepository.GetStatHolder(_hostBall);
        statHolder.TryGetStat(out Stat damage, StatType.Damage);
        float curseDamage = damage.CurrentValue * _config.CurseDamageMultiplier;

        _damageService.ApplyDamage(Host, entity, curseDamage, _config.CurseColor);
        _curseRegistry.RemoveCurse(enemy);
        enemy.EffectViewer.DisableEffect(EffectType.Curce);
    }
}