using System;
using BigBalls.GameplayObjects;

public interface IPlayerExperience
{
    Stat Stat { get; }
    float EXPForNextLevel { get; }
    float CurrentEXP { get; }

    event Action ValueChanged;
    event Action LevelUpped;
    void Init (Stat stat);
    void AddExperience(float value);
}
