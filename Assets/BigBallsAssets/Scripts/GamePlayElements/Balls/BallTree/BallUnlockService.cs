using System;
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

    public bool TryUnlockBall (BallModel ballModel)
    {
        BallType type = ballModel.BallConfig.BallType;
        var node = _ballsRepository.AllModels[type];

        if (CanUnlock(ballModel) == false)
            return false;

        if (_globalWallet.CanSpendCoins(ballModel.BallConfig.UnlockPrice) == false)
            return false;

        _globalWallet.TrySpendCoins(ballModel.BallConfig.UnlockPrice);

        if (_ballsRepository.AllModels.TryGetValue(type, out var ball))
        {
            ball.OpenItem();
            OnBallUnlocked?.Invoke(type);
            return true;
        }

        return false;
    }

    public bool CanUnlock (BallModel ballModel)
    {
        if (ballModel == null || ballModel.BallConfig == null)
            return false;

        if (ballModel.IsOpen)
            return false;

        var config = ballModel.BallConfig;
        BallType type = config.BallType;

        if (_ballsRepository.AllModels.TryGetValue(type, out var existing)
            && existing != null
            && existing.IsOpen)
            return false;

        if (config.ParentTypes != null && config.ParentTypes.Count > 0)
        {
            foreach (var parentType in config.ParentTypes)
            {
                if (!_ballsRepository.AllModels.TryGetValue(parentType, out var parentBall)
                    || parentBall == null)
                    return false;

                if (!parentBall.IsOpen)
                    return false;

                if (parentBall.ItemEXP < config.RequiredEXP)
                    return false;
            }
        }

        return true;
    }
}
