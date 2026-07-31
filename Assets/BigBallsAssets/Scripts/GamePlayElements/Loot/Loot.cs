using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public event Action<Loot> OnCollected;

    [field: SerializeField] public LootType Type { get; private set; }
    [field: SerializeField] public int  Value { get; private set; }

    public void Collect()
    {
        OnCollected?.Invoke(this);
    }
}