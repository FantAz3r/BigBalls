using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IBallFactory
    {
        event Action<Ball> BallReturned;
        Ball Create(BallConfig ballConfig, Transform parent = null);
    }
}