using BigBalls.Services;

public class WinService : IWinService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private IWinReason _winReason;
    private IWalletModel _walletModel;

    public WinService(IWindowService windowService, ITimeService timeService, IWalletModel walletModel)
    {
        _windowService = windowService;
        _timeService = timeService;
        _walletModel = walletModel;
    }

    public void SetWinReason(IWinReason winReason)
    {
        _winReason = winReason;
        _winReason.Won += OnWin;
    }

    public void OnWin()
    {
        _winReason.Won -= OnWin;
        _walletModel.PutAccumulatedCoins();
        _timeService.StopGame();
    }
}
