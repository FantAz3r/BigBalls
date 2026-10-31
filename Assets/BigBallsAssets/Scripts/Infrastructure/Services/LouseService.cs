using BigBalls.Infrastructure;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;

public class LouseService : ILouseService
{
    private readonly IWindowService _windowService;
    private readonly ITimeService _timeService;
    private readonly IUIFactory _uIFactory;
    private readonly LevelTimeline _levelTimeline;
    private ILouser _louser;
    private LevelID _levelID;

    public LouseService (IWindowService windowService, ITimeService timeService, IUIFactory uIFactory, LevelTimeline levelTimeline)
    {
        _windowService = windowService;
        _timeService = timeService;
        _uIFactory = uIFactory;
        _levelTimeline = levelTimeline;
    }

    public void SetLevel (LevelID level) => _levelID = level;

    public void SetLouseReason (ILouser louser)
    {
        _louser = louser;
        _louser.Died += OnLouse;
    }

    private void OnLouse ()
    {
        Debug.Log("louse");
        _louser.Died -= OnLouse;
        LouseLevelMenu louseLevelMenu = _windowService.Open<LouseLevelMenu>() as LouseLevelMenu;
        louseLevelMenu.LevelFailed(_levelID, _levelTimeline.CompliteWaves);
    }
}
