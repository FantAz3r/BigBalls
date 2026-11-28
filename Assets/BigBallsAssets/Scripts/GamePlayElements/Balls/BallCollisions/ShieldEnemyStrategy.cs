using BigBalls.GameplayObjects;
using UnityEngine;
using VContainer;

public class ShieldEnemyStrategy : ICollisionStrategy
{
    private IEntityRepository _entityRepository;

    [Inject]
    public void Construct(IEntityRepository entityRepository)
    {
        _entityRepository = entityRepository;
    }

    public bool HandleCollision (Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemy is not IceWizard)
            {
                Stat shield = enemy.StatHolder[StatType.Shield];
                StatHolder statHolder = _entityRepository.GetStatHolder(ball);
                statHolder.TryGetStat(out Stat damage, StatType.Damage);
                shield.AddCurrentValue(damage.CurrentValue);
                ball.EventHandler.Return(ball);
            }
        }

        return false;
    }

    public void HandleTrigger (Ball ball, Collider other)
    {
    }
}
