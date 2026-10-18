using BigBalls.Infrastructure;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;

public class LouseService : ILouseService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private readonly IUIFactory _uIFactory;
    private ILouser _louser;
    private LevelID _levelID;

    public LouseService(IWindowService windowService, ITimeService timeService, IUIFactory uIFactory)
    {
        _windowService = windowService;
        _timeService = timeService;
        _uIFactory = uIFactory;
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
        _uIFactory.Get<HUD>(WindowType.HUD).Close();
        _windowService.Open<LouseLevelMenu>();
    }
}
