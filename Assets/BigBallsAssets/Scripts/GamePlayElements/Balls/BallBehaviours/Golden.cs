using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Golden : EffectBehaviour
{
    private readonly Dictionary<IEntity, GoldenMark> _activeMarks = new();
    private readonly GoldenConfig _config;

    private LootSpawner _lootSpawner;
    private ICoroutineRunner _coroutineRunner;

    private class GoldenMark
    {
        public IEntity Enemy;
        public Action<IEntity> OnDeath;
        public Coroutine Coroutine;
        public Action Cancel;
    }

    public Golden (GoldenConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (LootSpawner lootSpawner, ICoroutineRunner coroutineRunner)
    {
        _lootSpawner = lootSpawner;
        _coroutineRunner = coroutineRunner;
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;

        return new DisposableObject(() =>
        {
            host.EventHandler.HitedEntity -= OnHit;
        });
    }

    private void OnHit (IEntity entity)
    {
        if (entity is not Enemy enemy)
            return;

        if (_activeMarks.TryGetValue(enemy, out var existing))
        {
            _activeMarks.Remove(enemy);
            ClearMark(existing);
        }

        ApplyMark(enemy);
    }

    private void ApplyMark (Enemy enemy)
    {
        void OnDeath (IEntity died)
        {
            enemy.EventHandler.Died -= OnDeath;
            _activeMarks.Remove(enemy);

            DropGold(enemy.transform.position);
        }

        enemy.EventHandler.Died += OnDeath;

        float duration = _config.GetMarkDuration(Level);
        var coroutine = _coroutineRunner.StartCoroutine(MarkRoutine(enemy, duration));

        var mark = new GoldenMark
        {
            Enemy = enemy,
            OnDeath = OnDeath,
            Coroutine = coroutine,
            Cancel = () =>
            {
                if (coroutine != null)
                    _coroutineRunner.StopCoroutine(coroutine);
            }
        };

        _activeMarks[enemy] = mark;

        enemy.ColorChanger?.SwapColor(_config.Color, _config.GetMarkDuration(Level));
    }

    private IEnumerator MarkRoutine (Enemy enemy, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (enemy == null || enemy.gameObject.activeSelf)
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_activeMarks.TryGetValue(enemy, out var mark))
        {
            _activeMarks.Remove(enemy);
            enemy.EventHandler.Died -= mark.OnDeath;
        }
    }

    private void DropGold (Vector3 position)
    {
        if (_config.GoldPrefab == null)
            return;

        int count = _config.LootPerDeath;
        for (int i = 0; i < count; i++)
            _lootSpawner.SpawnLootItem(position, _config.GoldPrefab);
    }


    private void ClearMark (GoldenMark mark)
    {
        if (mark.Enemy != null)
            mark.Enemy.EventHandler.Died -= mark.OnDeath;
        mark.Cancel?.Invoke();
    }
}