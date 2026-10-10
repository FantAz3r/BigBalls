using System;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Collider))]
public class Loot : MonoBehaviour
{
    private SoundSpawner _soundSpawner;

    public event Action<Loot> OnCollected;

    [field: SerializeField] public LootType Type { get; private set; }
    [field: SerializeField] public int Value { get; private set; }
    [field: SerializeField] public SoundStruct CollectSound { get; private set; }

    public Collider Collider { get; private set; }

    [Inject]
    public void Construct(SoundSpawner soundSpawner)
    {
        _soundSpawner = soundSpawner;
        Collider = GetComponent<Collider>();
    }

    public void Collect ()
    {
        OnCollected?.Invoke(this);
        _soundSpawner.Spawn(CollectSound);
    }
}