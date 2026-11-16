public interface IDebuffBehavior
{
    EffectType Type { get; }
    void Apply (StatHolder holder, DebuffHandle handle, float value);
}