using BigBalls.Services;

public class LootFactory : ILootFactory
{
    private IPoolService _poolService;
    private ILootMediator _lootMediator;
    private IUpdateService _updateService;
    private LootMover _lootMover;
    
    public LootFactory(IPoolService poolService, ILootMediator lootMediator, IUpdateService updateService)
    {
        _poolService = poolService;
        _lootMediator = lootMediator;
        _updateService = updateService;

        _lootMover = new LootMover(_updateService);
        _lootMover.Start();
        _lootMover.OnLootMissed += OnLootMissedHandler;
    }

    public void Disable()
    {
        _lootMover.Stop();
        _lootMover.OnLootMissed -= OnLootMissedHandler;
    }

    public Loot Create (Loot prefab)
    {
        Loot loot = _poolService.GetObject(prefab);
        loot.OnCollected += OnCollectedHandler;
        _lootMover.AddObject(loot);
        return loot;
    }

    private void OnCollectedHandler (Loot loot)
    {
        loot.OnCollected -= OnCollectedHandler;
        _lootMover.RemoveObject(loot);
        _lootMediator.RegisterLoot(loot);
        OnLootMissedHandler(loot);
    }

    private void OnLootMissedHandler(Loot loot)
    {
        loot.OnCollected -= OnCollectedHandler;
        _lootMover.OnLootMissed -= OnLootMissedHandler;
        _lootMover.RemoveObject(loot);
        _poolService.ReleaseObject(loot);
    }
}
