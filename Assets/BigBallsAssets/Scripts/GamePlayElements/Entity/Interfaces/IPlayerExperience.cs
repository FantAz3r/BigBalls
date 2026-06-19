using System;
using BigBalls.GameplayObjects;

public interface IPlayerExperience
{
    float MaxValue { get; }
    float CurrentValue { get; }

    event Action<IPlayerExperience> OnValueChanged;
    void Init(Stat stat);
    void AddExperience(float value);
}
