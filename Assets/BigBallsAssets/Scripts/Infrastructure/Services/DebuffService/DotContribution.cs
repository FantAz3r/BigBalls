using System;
using BigBalls.GameplayObjects;
using UnityEngine;

public sealed class DotContribution
{
    public float Dps { get; }
    public Color Color { get; }
    public float Remaining { get; set; }
    public float TickAccumulator { get; set; }
    public float AccumulatedDamage { get; set; }
    public bool Finished { get; private set; }

    private Action<DebuffResult> _onFinished;
    private readonly IEntity _entity;
    private readonly EffectType _type;

    public DotContribution (float dps, Color color, float duration, IEntity entity, EffectType type, Action<DebuffResult> onFinished)
    {
        Dps = dps;
        Color = color;
        Remaining = duration;
        _entity = entity;
        _type = type;
        _onFinished = onFinished;
    }

    public void Finish (bool cancelled)
    {
        if (Finished)
            return;

        Finished = true;
        _onFinished?.Invoke(new DebuffResult(_entity, _type, AccumulatedDamage, cancelled));
        _onFinished = null;
    }
}