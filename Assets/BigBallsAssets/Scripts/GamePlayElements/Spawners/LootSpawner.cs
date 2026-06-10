using System.Collections;
using System.Collections.Generic;
using BigBalls.Services;
using BigBalls.StaticData;
using UnityEngine;

public class LootSpawner
{
    private ILootFactory _lootFactory;
    
    public LootSpawner(ILootFactory lootFactory)
    {
        _lootFactory = lootFactory;
    }
    
    
    public void SpawnLootItem(Vector3 position, LootInfo lootInfo)
    {
        // Получаем объект из пула
        Loot loot = _lootFactory.Create(lootInfo);
        
        loot.transform.position = position + Random.insideUnitSphere * 0.5f;
        
        // Инициализируем компонент лута
        // loot.Initialize(lootInfo.LootType, GetValueForLootType(lootInfo));
    }
    
    public void DropBaseRewards(Vector3 position, EnemyConfig config)
    {
        Debug.Log($"Игроку назначено {config.BaseCoins} очков опыта");
        
        // Опыт автоматически даётся игроку
        // var player = GameObject.FindGameObjectWithTag("Player");
        // player?.GetComponent<ILevelable>()?.AddExperience(config.BaseExperience);
        
        // Монеты спавнятся как объекты
        for (int i = 0; i < config.BaseCoins; i++)
        {
            // Спавн монеты с небольшим разбросом
            // SpawnCoin(position + Random.insideUnitSphere * 0.3f);
        }
    }
    
    private int GetValueForLootType(LootInfo info)
    {
        switch (info.LootType)
        {
            case LootType.Coin:
                return Random.Range(5, 20);
            case LootType.Experience:
                return Random.Range(10, 30);
            case LootType.HealthPotion:
                return 25; // восстанавливает 25 HP
            default:
                return 1;
        }
    }
}
