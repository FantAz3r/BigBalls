using BigBalls.Services;
using BigBalls.UI;

public class LouseService : ILouseService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private ILouser _louser;

    public LouseService(IWindowService windowService, ITimeService timeService)
    {
        _windowService = windowService;
        _timeService = timeService;
    }

    public void SetLouseReason(ILouser louser)
    {
        _louser = louser;
        _louser.Lost += OnLouse;
    }

    private void OnLouse()
    {
        _louser.Lost -= OnLouse;
        _timeService.StopGame();
        _windowService.Open<LouseLevelMenu>();
    }
}
