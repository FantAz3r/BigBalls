using BigBalls.Configs;
using BigBalls.GameplayObjects;
using System;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IBallFactory
    {
        event Action BallReturned;
        Ball Create(BallConfig ballConfig, int level = 0);
    }
}