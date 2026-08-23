using UnityEngine;

public class LootSpawner
{
    private ILootFactory _lootFactory;

    public LootSpawner (ILootFactory lootFactory)
    {
        _lootFactory = lootFactory;
    }

    public void SpawnLootItem (Vector3 position, Loot prefab)
    {
        Loot loot = _lootFactory.Create(prefab.name);
        Vector3 randomOffset = Random.insideUnitCircle * 0.5f;
        loot.transform.position = position + new Vector3(randomOffset.x, 0, randomOffset.y);
    }
}