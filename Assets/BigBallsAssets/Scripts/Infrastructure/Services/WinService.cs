using BigBalls.Configs;
using BigBalls.Infrastructure;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;

public class WinService : IWinService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private readonly IResourceLoader _resourceLoader;
    private IWinReason _winReason;
    private IWalletModel _walletModel;
    private LevelID _level;
    private LevelConfig _levelConfig;

    public WinService(
        IWindowService windowService,
        ITimeService timeService,
        IWalletModel walletModel,
        IResourceLoader resourceLoader)
    {
        _windowService = windowService;
        _timeService = timeService;
        _walletModel = walletModel;
        _resourceLoader = resourceLoader;
    }

    public void SetLevel(LevelID level)
    {
        Debug.Log(level);
        _level = level;
        _levelConfig = _resourceLoader.Load<LevelData>().Get(_level);
    }

    public void SetWinReason(IWinReason winReason)
    {
        _winReason = winReason;
        _winReason.Won += OnWin;
    }

    public void OnWin()
    {
        _winReason.Won -= OnWin;
        WinLevelMenu winMenu = _windowService.Open<WinLevelMenu>() as WinLevelMenu;
        winMenu.LevelComplite(_level, _levelConfig.Waves.Count);

        _walletModel.PutAccumulatedCoins();
        _timeService.StopGame();
    }
}
