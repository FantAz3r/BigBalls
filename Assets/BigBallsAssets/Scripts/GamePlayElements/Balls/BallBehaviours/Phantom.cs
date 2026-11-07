using System;
using System.Collections.Generic;
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

    private readonly Dictionary<IEntity, CurseState> _curses = new();

    private class CurseState
    {
        public int Hits;
        public Action<IEntity> OnDeath;
        public Action<IEntity> OnHitCheck;
        public Action Cancel;
    }

    public Phantom (PhantomConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (IEntityRepository entityRepository, IDamageService damageService)
    {
        _entityRepository = entityRepository;
        _damageService = damageService;
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

        if (_curses.TryGetValue(enemy, out var state) == false)
        {
            state = new CurseState();
            state.OnDeath = _ => RemoveCurse(enemy);
            enemy.EventHandler.Died += state.OnDeath;
            _curses[enemy] = state;
        }

        state.Hits++;

        int needed = _config.GetHitsToProc(Level);
        if (state.Hits < needed)
            return;

        StatHolder statHolder = _entityRepository.GetStatHolder(_hostBall);
        statHolder.TryGetStat(out Stat damage, StatType.Damage);
        float curceDamage = damage.CurrentValue * _config.CurseDamageMultiplier;

        _damageService.ApplyDamage(Host, entity, curceDamage, _config.CurseColor);
        RemoveCurse(enemy);
    }

    private void RemoveCurse (IEntity enemy)
    {
        if (enemy == null)
            return;

        if (_curses.TryGetValue(enemy, out var state) == false)
            return;

        _curses.Remove(enemy);
    }
}