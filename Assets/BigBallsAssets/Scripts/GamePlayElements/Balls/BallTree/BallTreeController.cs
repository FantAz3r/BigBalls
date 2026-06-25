using System;
using BigBalls.StaticData;
using UnityEngine;

public class BallTreeController
{
    private readonly BallTreeModel _treeModel;
    private readonly BallsRepository _ballRepository;
    private readonly GlobalWallet _wallet;
    private readonly IBallUnlockService _unlockService;

    public event Action<BallType> OnBallUnlocked;
    public event Action<BallType> OnBallUpgraded;

    public BallTreeController (
        BallTreeModel treeModel,
        BallsRepository ballRepository,
        GlobalWallet wallet,
        IBallUnlockService unlockService)
    {
        _ballRepository = ballRepository;
        _treeModel = treeModel;
        _wallet = wallet;
        _unlockService = unlockService;
    }

    public bool CanUnlockBall (BallType type)
    {
        return _treeModel.CanUnlock(type);
    }

    public bool TryUnlockBall (BallType type)
    {
        if (CanUnlockBall(type) == false)
            return false;

        if (_unlockService.UnlockBall(type))
        {
            if (_ballRepository.AllModels.TryGetValue(type, out var ball))
            {
                OnBallUnlocked?.Invoke(type);
                return true;
            }
        }

        return false;
    }

    public BallTreeModel GetTreeModel () => _treeModel;

    // public void AddExperienceToBall (BallType type, float exp)
    // {
    //     if (_ballRepository.AllModels.TryGetValue(type, out var ball) == false)
    //         return;
    //
    //     ball.AddItemEXP(exp);
    //
    //     // Проверяем, можно ли повысить уровень
    //     if (ball.ItemEXP >= GetExpToNextLevel(ball.Level))
    //     {
    //         ball.Upgrade();
    //         OnBallUpgraded?.Invoke(type);
    //     }
    // }
    //
    // private float GetExpToNextLevel (int currentLevel)
    // {
    //     // Формула опыта для уровня
    //     return 100f + currentLevel * 50f;
    // }
}
