using System.Collections.Generic;
using BigBalls.GameplayObjects;
using UnityEngine;

public sealed class DebuffHandle
{
    private readonly List<(Stat stat, float removed)> _restore = new();
    private readonly EffectViewer _visual;

    public IEntity Entity { get; }
    public EffectType Type { get; }
    public bool Cancelled { get; private set; }
    public bool Finished { get; set; }
    public float Remaining { get; set; }
    public List<DotContribution> Contributions { get; } = new();
    public bool HasContributions => Contributions.Count > 0;

    public DebuffHandle (IEntity entity, EffectType type)
    {
        Entity = entity;
        Type = type;
        Entity.Transform.TryGetComponent(out EffectViewer effectViewer);
        _visual = effectViewer;
        _visual?.EnableEffect(type);
    }

    public void Remember (Stat stat, float removed)
    {
        if (removed > 0f)
            _restore.Add((stat, removed));
    }

    public void Cancel () => Cancelled = true;

    public void AddContribution (DotContribution c) => Contributions.Add(c);

    public void RemoveFinishedContributions ()
    {
        for (int i = Contributions.Count - 1; i >= 0; i--)
            if (Contributions[i].Finished)
                Contributions.RemoveAt(i);
    }

    public void Restore ()
    {
        _visual?.DisableEffect(Type);
        foreach (var (stat, removed) in _restore)
            stat.AddCurrentValue(removed);
        _restore.Clear();
    }
}
