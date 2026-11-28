using BigBalls.Configs;
using BigBalls.Infrastructure;
using BigBalls.Services;
using BigBalls.UI;

public class WinService : IWinService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private readonly IInputService _inputService;
    private IWinReason _winReason;
    private LevelConfig _levelConfig;


    public WinService (
        IWindowService windowService,
        ITimeService timeService,
        InputServiceAdapter inputServiceAdapter)
    {
        _windowService = windowService;
        _timeService = timeService;
        _inputService = inputServiceAdapter.Get();
    }

    public void SetLevelConfig (LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;
    }

    public void SetWinReason (IWinReason winReason)
    {
        _winReason = winReason;
        _winReason.Won += OnWin;
    }

    public void OnWin ()
    {
        _inputService.DisablePlayer();
        _winReason.Won -= OnWin;
        WinLevelMenu winMenu = _windowService.Open<WinLevelMenu>() as WinLevelMenu;
        winMenu.LevelComplite(_levelConfig, _levelConfig.GetBossCount());
        _timeService.StopGame();
    }
}
