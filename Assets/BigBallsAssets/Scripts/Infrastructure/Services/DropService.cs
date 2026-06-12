using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

public class DropService : IDropService
{
    private readonly LootSpawner _lootSpawner;
    
    private Dictionary<string, EnemyConfig> _enemyConfigs = new Dictionary<string, EnemyConfig>();

    public DropService(LootSpawner lootSpawner)
    {
        _lootSpawner = lootSpawner;
    }
    
    public void DropLoot(Vector3 position, Enemy enemy)
    {
        EnemyConfig config = _enemyConfigs[enemy.name];
        
        foreach (var lootInfo in config.PossibleLoot)
        {
            // Проверка шанса
            if (Random.value > lootInfo.DropChance)
                continue;
            
            int amount = Random.Range(lootInfo.MinAmount, lootInfo.MaxAmount + 1);
            
            Debug.Log($"Игроку выпало {lootInfo.LootPrefab} в количестве {amount}");
            
            for (int i = 0; i < amount; i++)
            {
                _lootSpawner.SpawnLootItem(position, lootInfo);
            }
        }
        
        // Базовый дроп всегда выпадает
        _lootSpawner.DropBaseRewards(position, config);
    }

    public void SetCurrentLevelConfig(List<EnemyConfig> currentEnemyConfigs)
    {
        foreach (EnemyConfig config in currentEnemyConfigs)
        {
            _enemyConfigs.TryAdd(config.name, config);
        }
    }
}
