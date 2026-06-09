using BigBalls.Services;

public class WinService : IWinService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private IWinReason _winReason;

    public WinService(IWindowService windowService, ITimeService timeService)
    {
        _windowService = windowService;
        _timeService = timeService;
    }

    public void SetWinReason(IWinReason winReason)
    {
        _winReason = winReason;
        _winReason.Won += OnWin;
    }

    public void OnWin()
    {
        _winReason.Won -= OnWin;
        _timeService.StopGame();
    }
}
