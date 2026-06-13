using BigBalls.Configs;
using BigBalls.GameplayObjects;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IEnemyFactory
    {
        Enemy Create(EnemyConfig enemyConfig, Vector3 position);
    }
}