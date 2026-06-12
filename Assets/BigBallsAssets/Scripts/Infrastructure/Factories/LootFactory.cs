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

        loot.Collected += OnLootHandler;

        return loot;
    }

    private void OnLootHandler(Loot loot)
    {
        loot.Collected -= OnLootHandler;
        
        _poolService.ReleaseObject(loot);
    }
}
