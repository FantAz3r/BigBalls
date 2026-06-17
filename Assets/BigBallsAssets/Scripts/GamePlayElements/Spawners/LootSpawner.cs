using UnityEngine;

public class LootSpawner
{
    private ILootFactory _lootFactory;

    public LootSpawner (ILootFactory lootFactory)
    {
        _lootFactory = lootFactory;
    }

    public void SpawnLootItem (Vector3 position, LootInfo lootInfo)
    {
        Loot loot = _lootFactory.Create(lootInfo.LootPrefab.name);
        Vector3 randomOffset = Random.insideUnitCircle * 0.5f;
        loot.transform.position = position + new Vector3(randomOffset.x, 0, randomOffset.y);
        loot.Initialize(lootInfo.LootType, Random.Range(lootInfo.MinAmount, lootInfo.MaxAmount + 1));
    }
}
