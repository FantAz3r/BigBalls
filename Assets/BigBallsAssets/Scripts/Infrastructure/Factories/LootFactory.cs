using System.Collections;
using System.Collections.Generic;
using BigBalls.Services;
using UnityEngine;

public class LootFactory : ILootFactory
{
    private IPoolService _poolService;
    
    public LootFactory(IPoolService poolService)
    {
        _poolService = poolService;
    }

    public Loot Create(LootInfo lootInfo)
    {
        string lootName = lootInfo.LootPrefab.name;

        Loot loot = _poolService.GetObject<Loot>(lootName);

        return loot;
    }
}
