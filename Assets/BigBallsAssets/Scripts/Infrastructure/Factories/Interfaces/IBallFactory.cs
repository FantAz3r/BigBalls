using BigBalls.GameplayObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Factories
{
    public interface IBallFactory
    {
        event Action<BallModel> BallReturned;
        Ball Create (BallModel ballModel, EntityType type, Transform shooter, List<ICollisionStrategy> strategies, Quaternion rotation = default);
    }
}