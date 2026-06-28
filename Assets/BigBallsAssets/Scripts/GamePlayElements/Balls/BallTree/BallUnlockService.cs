using System;
using System.Collections.Generic;
using BigBalls.StaticData;

public class BallUnlockService : IBallUnlockService
{
    private readonly BallRepository _ballsRepository;
    private readonly GlobalWallet _globalWallet;

    public BallUnlockService (BallRepository ballsRepository, GlobalWallet globalWallet)
    {
        _ballsRepository = ballsRepository;
        _globalWallet = globalWallet;
    }

    public event Action<BallType> OnBallUnlocked;
    public event Action<BallType> OnBallUpgraded;

    public bool TryUnlockBall (BallType type)
    {
        if (CanUnlock(type) == false)
            return false;


        if (_ballsRepository.AllModels.TryGetValue(type, out var ball))
        {
            ball.OpenItem();
            OnBallUnlocked?.Invoke(type);
            return true;
        }

        return false;
    }

    public bool CanUnlock (BallType type)
    {
        var node = _ballsRepository.AllModels[type];

        if (node == null)
            return false;

        if (_ballsRepository.AllModels.TryGetValue(type, out var ball) && ball.IsOpen)
            return false;

        foreach (var parentType in node.BallConfig.ParentTypes ?? new List<BallType>())
        {
            if (_ballsRepository.AllModels.TryGetValue(parentType, out var parentBall) == false)
                return false;

            if (parentBall.IsOpen == false)
                return false;

            if (parentBall.ItemEXP < node.BallConfig.RequiredEXP)
                return false;
        }

        return true;
    }
}
