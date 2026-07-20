using BigBalls.Configs;
using BigBalls.GameplayObjects;
using System;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IEnemyFactory
    {
        event Action<IEntity> Died;

        Enemy Create(EnemyConfig enemyConfig, Vector3 position);
    }
}