using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IEnemyFactory
    {
        Enemy Create(EnemyConfig enemyConfig, Vector3 position);
    }
}