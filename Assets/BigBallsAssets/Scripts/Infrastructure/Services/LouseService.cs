using BigBalls.Infrastructure;
using BigBalls.Services;
using BigBalls.UI;

public class LouseService : ILouseService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private ILouser _louser;
    private LevelID _levelID;

    public LouseService(IWindowService windowService, ITimeService timeService)
    {
        _windowService = windowService;
        _timeService = timeService;
    }

    public void SetLevel(LevelID level) => _levelID = level;

    public void SetLouseReason(ILouser louser)
    {
        _louser = louser;
        _louser.Died += OnLouse;
    }

    private void OnLouse()
    {
        _louser.Died -= OnLouse;
        _timeService.StopGame();
        _windowService.Open<LouseLevelMenu>();
    }
}
