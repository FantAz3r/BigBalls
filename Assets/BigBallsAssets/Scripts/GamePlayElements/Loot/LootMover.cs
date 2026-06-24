using System;
using System.Collections.Generic;
using BigBalls.Services;
using UnityEngine;

public class LootMover: IUpdateble
{
    private const float _speed = 0.2f;
    private const int LevelEndZ = -12;
    
    private readonly IUpdateService _updateService;

    private List<Loot> _activeLoot;

    public event Action<Loot> OnLootMissed; 

    public LootMover(IUpdateService updateService, List<Loot> activeLoot)
    {
        _updateService = updateService;
        _activeLoot = activeLoot;
    }
    
    public void Start() => _updateService.Register(this);
    
    public void Stop() => _updateService.Unregister(this);

    public void Tick()
    {
        for (int i = _activeLoot.Count - 1; i >= 0; i--)
        {
            Loot loot = _activeLoot[i];
            
            if (loot == null) continue;
            
            loot.transform.Translate(Vector3.back * _speed * Time.deltaTime);

            if (loot.transform.position.z <= LevelEndZ)
            {
                OnLootMissed?.Invoke(loot);
            }
        }
    }
}
