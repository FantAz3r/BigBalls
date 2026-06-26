using System;
using BigBalls.StaticData;

public interface IBallUnlockService
{
    event Action<BallType> OnBallUpgraded;
    event Action<BallType> OnBallUnlocked;

    bool CanUnlock (BallType type);
    bool TryUnlockBall (BallType type);
}
