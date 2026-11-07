using BigBalls.GameplayObjects;

public readonly struct DebuffResult
{
    public readonly IEntity Entity;
    public readonly EffectType Type;
    public readonly float Damage;
    public readonly bool Cancelled;

    public DebuffResult (IEntity entity, EffectType type, float damage, bool cancelled)
    {
        Entity = entity;
        Type = type;
        Damage = damage;
        Cancelled = cancelled;
    }
}