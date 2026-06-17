using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using UnityEngine;

public class DropService : IDropService
{
    private readonly LootSpawner _lootSpawner;

    private Dictionary<string, EnemyConfig> _enemyConfigs = new Dictionary<string, EnemyConfig>();

    public DropService (LootSpawner lootSpawner)
    {
        _lootSpawner = lootSpawner;
    }

    public void DropLoot (Vector3 position, Enemy enemy)
    {
        EnemyConfig config = _enemyConfigs[enemy.name];

        foreach (var lootInfo in config.PossibleLoot)
        {
            if (Random.value > lootInfo.DropChance)
                continue;

            _lootSpawner.SpawnLootItem(position, lootInfo);
        }
    }

    public void SetCurrentLevelConfig (List<EnemyConfig> currentEnemyConfigs)
    {
        foreach (EnemyConfig config in currentEnemyConfigs)
        {
            _enemyConfigs.TryAdd(config.name, config);
        }
    }
}
