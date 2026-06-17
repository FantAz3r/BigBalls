using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private LootType _type;
    private int _value;
    
    public event Action<Loot> OnCollected;

    public LootType Type => _type;
    public int Value => _value;
    
    public void Initialize(LootType type, int value)
    {
        _type = type;
        _value = value;
    }

    public void Collect()
    {
        OnCollected?.Invoke(this);
    }
}