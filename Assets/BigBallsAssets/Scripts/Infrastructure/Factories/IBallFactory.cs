using BigBalls.GameplayObjects;
using System;

namespace BigBalls.Factories
{
    public interface IBallFactory
    {
        event Action BallReturned;
        Ball Create (BallModel ballModel);
    }
}