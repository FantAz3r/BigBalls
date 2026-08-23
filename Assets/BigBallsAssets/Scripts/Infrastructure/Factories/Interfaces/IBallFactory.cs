using BigBalls.GameplayObjects;
using System;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IBallFactory
    {
        event Action<BallModel> BallReturned;
        Ball Create(BallModel ballModel, EntityType type, Transform shooter, Quaternion rotation = default);
    }
}