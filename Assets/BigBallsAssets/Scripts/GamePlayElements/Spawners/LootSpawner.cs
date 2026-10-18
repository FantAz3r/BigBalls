using UnityEngine;

public class LootSpawner
{
    private const float OffsetY = 0.3f;
    private ILootFactory _lootFactory;

    public LootSpawner (ILootFactory lootFactory)
    {
        _lootFactory = lootFactory;
    }

    public void SpawnLootItem (Vector3 position, Loot prefab)
    {
        Loot loot = _lootFactory.Create(prefab);
        Vector3 randomOffset = Random.insideUnitCircle * 0.5f;
        position.y = OffsetY;
        loot.transform.position = position + new Vector3(randomOffset.x, 0, randomOffset.y);
    }
}