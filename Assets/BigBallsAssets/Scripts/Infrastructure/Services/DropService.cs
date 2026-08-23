using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using System.Collections.Generic;
using UnityEngine;

public class DropService : IDropService
{
    private readonly LootSpawner _lootSpawner;
    private readonly IResourceLoader _resourceLoader;
    private readonly LootData _lootData;

    private Dictionary<string, EnemyConfig> _enemyConfigs = new Dictionary<string, EnemyConfig>();

    public DropService(LootSpawner lootSpawner, IResourceLoader resourceLoader)
    {
        _lootSpawner = lootSpawner;
        _lootData = resourceLoader.Load<LootData>();
    }

    public void DropLoot(Vector3 position, Enemy enemy)
    {
        EnemyConfig config = _enemyConfigs[enemy.name];

        foreach (var lootInfo in config.PossibleLoot)
        {
            if (Random.value > lootInfo.DropChance)
                continue;

            int count = Random.Range(lootInfo.MinAmount, lootInfo.MaxAmount);

            foreach (var loot in _lootData.GetLoot(lootInfo.LootType, count))
            {
                _lootSpawner.SpawnLootItem(position, loot.Prefab);

            }
        }
    }

    public void SetCurrentLevelConfig(List<EnemyConfig> currentEnemyConfigs)
    {
        foreach (EnemyConfig config in currentEnemyConfigs)
        {
            _enemyConfigs.TryAdd(config.name, config);
        }
    }
}
