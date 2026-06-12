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
        
        Vector3 randomOffset = Random.insideUnitCircle * 0.5f;
        loot.transform.position = position + new Vector3(randomOffset.x, 0, randomOffset.y);
        
        // Инициализируем компонент лута
        loot.Initialize(lootInfo.LootType, GetValueForLootType(lootInfo));
    }
    
    public void DropBaseRewards(Vector3 position, EnemyConfig config)
    {
        Debug.Log($"Игроку назначено {config.BaseCoins} очков опыта");
        
        // Опыт автоматически даётся игроку
        
        // Монеты спавнятся как объекты - дополнительно рандомно с каждого убитого противника?
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
                return 25;                     // восстанавливает 25 HP
            default:
                return 1;
        }
    }
}
