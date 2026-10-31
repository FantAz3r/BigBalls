using System;
using BigBalls.StaticData;

public interface IBallUnlockService
{
    event Action<BallType> OnBallUpgraded;
    event Action<BallType> OnBallUnlocked;

    bool CanUnlock (BallModel ballModel);
    bool TryUnlockBall (BallModel ballModel);
}
