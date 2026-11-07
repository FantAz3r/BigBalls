using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class EnemyCollisionStrategy : ICollisionStrategy
{
    private IEntityRepository _entityRepository;
    private IDamageService _damageService;

    [Inject]
    public void Construct(IEntityRepository entityRepository, IDamageService damageService)
    {
        _entityRepository = entityRepository;
        _damageService = damageService;

    }

    public bool HandleCollision (Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Shield>(out _))
        {
            return false;
        }

        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemy == null)
                return false;

            OnHit(enemy, ball);
        }
        else if (collision.gameObject.TryGetComponent(out EnemyPart component))
        {
            if (component.Enemy == null)
                return false;

            OnHit(component.Enemy, ball);
        }


        ball.EventHandler.Hit();
        return false;
    }

    private void OnHit (Enemy enemy, Ball ball)
    {
        StatHolder statHolder = _entityRepository.GetStatHolder(ball);

        if (statHolder.TryGetStat(out Stat stat, StatType.Damage))
        {
            float damage =_damageService.ApplyDamage( ball, enemy, stat.CurrentValue, ball.Config.DamageColor);
            ball.CalculateDamage(damage);
        }

        ball.SetCanReturnToBag(true);
        ball.EventHandler.HitEntity(enemy);

        if (enemy.EnemyHitedAnimation != null)
        {
            enemy.EnemyHitedAnimation.OnHit(ball);
        }
    }

    public void HandleTrigger (Ball ball, Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            OnHit(enemy, ball);
        }
    }
}
