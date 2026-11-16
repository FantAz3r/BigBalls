using System.Collections.Generic;
using BigBalls.GameplayObjects;

public sealed class DebuffRegistry
{
    private readonly Dictionary<(IEntity, EffectType), DebuffHandle> _active = new();

    public bool TryGet (IEntity entity, EffectType type, out DebuffHandle handle)
        => _active.TryGetValue((entity, type), out handle);

    public void Add (DebuffHandle handle) => _active[(handle.Entity, handle.Type)] = handle;

    public void RemoveSame (DebuffHandle handle)
    {
        var key = (handle.Entity, handle.Type);
        if (_active.TryGetValue(key, out var current) && current == handle)
            _active.Remove(key);
    }
}