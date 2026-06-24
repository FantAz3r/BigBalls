using System;
using System.Collections.Generic;
using BigBalls.Services;

public class LootFactory : ILootFactory, IDisposable
{
    private IPoolService _poolService;
    private ILootMediator _lootMediator;
    private IUpdateService _updateService;

    private List<Loot> _activLoots = new List<Loot>();

    private LootMover _lootMover;
    
    public LootFactory(IPoolService poolService, ILootMediator lootMediator, IUpdateService updateService)
    {
        _poolService = poolService;
        _lootMediator = lootMediator;
        _updateService = updateService;

        _lootMover = new LootMover(_updateService, _activLoots);
        _lootMover.Start();
        _lootMover.OnLootMissed += OnLootMissedHandler;
    }

    public void Dispose()
    {
        _activLoots.Clear();
        _lootMover.OnLootMissed -= OnLootMissedHandler;
        _lootMover.Stop();
    }

    public Loot Create(string name)
    {
        Loot loot = _poolService.GetObject<Loot>(name);
        loot.OnCollected += OnCollectedHandler;
        _activLoots.Add(loot);

        return loot;
    }

    private void OnCollectedHandler(Loot loot)
    {
        loot.OnCollected -= OnCollectedHandler;
        
        _activLoots.Remove(loot);
        _lootMediator.RegisterLoot(loot);
        _poolService.ReleaseObject(loot);
    }
    
    
    private void OnLootMissedHandler(Loot loot)
    {
        if (_activLoots.Contains(loot))
        {
            _activLoots.Remove(loot);
        }
        
        _poolService.ReleaseObject(loot);
    }
}
