using BigBalls.Configs;
using BigBalls.GameplayObjects;
using System;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IEnemyFactory
    {
        bool HasAliveEnemies { get; }
        int DiedCount { get; }
        int SuisidedCount { get; }

        event Action<IEntity> Died;
        event Action<IEntity> Killed;
        event Action BossKilled;

        Enemy Create(EnemyConfig enemyConfig, Vector3 position);
    }
}