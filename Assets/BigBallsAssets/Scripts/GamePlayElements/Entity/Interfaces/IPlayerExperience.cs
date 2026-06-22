using System;
using BigBalls.GameplayObjects;

public interface IPlayerExperience
{
    event Action<IPlayerExperience> ValueChanged;
    void Init(Stat stat);
    void AddExperience(float value);
}
