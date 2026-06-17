using BigBalls.Services;

public class LootFactory : ILootFactory
{
    private IPoolService _poolService;
    private ILootMediator _lootMediator;
    
    public LootFactory(IPoolService poolService, ILootMediator lootMediator)
    {
        _poolService = poolService;
        _lootMediator = lootMediator;
    }

    public Loot Create(string name)
    {
        Loot loot = _poolService.GetObject<Loot>(name);
        loot.OnCollected += OnCollectedHandler;

        return loot;
    }

    private void OnCollectedHandler(Loot loot)
    {
        loot.OnCollected -= OnCollectedHandler;
        
        _lootMediator.RegisterLoot(loot);
        _poolService.ReleaseObject(loot);
    }
}
