using BigBalls.StaticData;

namespace BigBalls.Factories
{
    public interface IEnemyFactory
    {
        Enemy Create(EnemyConfig enemyConfig);
    }
}