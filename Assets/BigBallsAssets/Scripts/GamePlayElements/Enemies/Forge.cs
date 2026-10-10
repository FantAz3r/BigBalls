using System.Collections;
using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Forge : Enemy
{
    private ICoroutineRunner _coroutineRunner;
    private EnemySpawner _enemySpawner;
    private IEnemyFactory _enemyFactory;

    [Inject]
    public void Construct (EnemySpawner enemySpawner, ICoroutineRunner coroutineRunner, IEnemyFactory enemyFactory)
    {
        _coroutineRunner = coroutineRunner;
        _enemySpawner = enemySpawner;
        _enemyFactory = enemyFactory;
    }

    protected override void ChildInit ()
    {

    }
}
