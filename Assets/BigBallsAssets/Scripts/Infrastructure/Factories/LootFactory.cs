using BigBalls.Services;

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

        loot.OnLoot += OnLootHandler;

        return loot;
    }

    private void OnLootHandler(Loot loot)
    {
        loot.OnLoot -= OnLootHandler;
        
        _poolService.ReleaseObject(loot);
    }
}
