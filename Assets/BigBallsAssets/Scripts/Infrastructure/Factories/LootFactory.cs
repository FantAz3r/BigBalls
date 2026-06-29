using System;
using System.Collections.Generic;
using BigBalls.Services;

public class LootFactory : ILootFactory, IDisposable
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

    public void Dispose()
    {
        _lootMover.OnLootMissed -= OnLootMissedHandler;
        _lootMover.Stop();
    }

    public Loot Create (string name)
    {
        Loot loot = _poolService.GetObject<Loot>(name);
        loot.OnCollected += OnCollectedHandler;
        _lootMover.AddObject(loot);
        return loot;
    }

    private void OnCollectedHandler (Loot loot)
    {
        loot.OnCollected -= OnCollectedHandler;

        _lootMediator.RegisterLoot(loot);
        _poolService.ReleaseObject(loot);
    }

    private void OnLootMissedHandler(Loot loot)
    {
        _poolService.ReleaseObject(loot);
    }
}
