using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;

public class CurseRegistry
{
    private readonly Dictionary<IEntity, CurseEntry> _curses = new();

    public event Action<IEntity, int> HitAdded;
    public event Action<IEntity> Removed;

    private class CurseEntry
    {
        public int Hits;
        public Action<IEntity> OnDeath;
    }

    public bool IsCursed (IEntity entity) => _curses.ContainsKey(entity);

    public int GetHits (IEntity entity)
        => _curses.TryGetValue(entity, out var e) ? e.Hits : 0;

    public bool EnsureCurse (IEntity entity, Action<IEntity> onDeath)
    {
        if (_curses.TryGetValue(entity, out var existing))
        {
            return false;
        }

        var entry = new CurseEntry
        {
            Hits = 0,
            OnDeath = onDeath,
        };

        entity.EventHandler.Died += onDeath;
        _curses[entity] = entry;
        return true;
    }

    public int AddHit (IEntity entity)
    {
        if (!_curses.TryGetValue(entity, out var entry))
            return 0;

        entry.Hits++;
        HitAdded?.Invoke(entity, entry.Hits);
        return entry.Hits;
    }

    public bool RemoveCurse (IEntity entity)
    {
        if (!_curses.TryGetValue(entity, out var entry))
            return false;

        entity.EventHandler.Died -= entry.OnDeath;
        _curses.Remove(entity);
        Removed?.Invoke(entity);
        return true;
    }

    public void Clear ()
    {
        foreach (var kv in _curses)
            kv.Key.EventHandler.Died -= kv.Value.OnDeath;

        _curses.Clear();
    }
}