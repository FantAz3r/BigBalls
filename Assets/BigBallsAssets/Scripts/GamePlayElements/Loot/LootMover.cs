using System;
using System.Collections.Generic;
using BigBalls.Services;
using UnityEngine;

public class LootMover: IUpdateble
{
    private const float _speed = 0.2f;
    private const int LevelEndZ = -12;
    
    private readonly IUpdateService _updateService;

    private List<Loot> _activeLoots = new List<Loot>();

    public event Action<Loot> OnLootMissed; 

    public LootMover(IUpdateService updateService)
    {
        _updateService = updateService;
    }
    
    public void Start() => _updateService.Register(this);
    
    public void Stop()
    {
        _activeLoots.Clear();
        _updateService.Unregister(this);
    } 

    public void Tick()
    {
        for (int i = _activeLoots.Count - 1; i >= 0; i--)
        {
            Loot loot = _activeLoots[i];

            if (loot.gameObject.activeInHierarchy == false)
            {
                _activeLoots.Remove(loot);
                continue;
            }
            
            loot.transform.Translate(Vector3.back * _speed * Time.deltaTime);

            if (loot.transform.position.z <= LevelEndZ)
            {
                _activeLoots.Remove(loot);
                OnLootMissed?.Invoke(loot);
            }
        }
    }
    
    public void AddObject(Loot loot)
    {
        _activeLoots.Add(loot);
    }
}
