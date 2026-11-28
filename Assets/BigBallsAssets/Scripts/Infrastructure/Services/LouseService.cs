using BigBalls.Configs;
using BigBalls.Infrastructure;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;

public class LouseService : ILouseService
{
    private readonly IWindowService _windowService;
    private readonly LevelTimeline _levelTimeline;
    private ILouser _louser;
    private LevelConfig _levelConfig;

    public LouseService (IWindowService windowService, LevelTimeline levelTimeline)
    {
        _windowService = windowService;
        _levelTimeline = levelTimeline;
    }

    public void SetLevelConfig (LevelConfig levelConfig) => _levelConfig = levelConfig;

    public void SetLouseReason (ILouser louser)
    {
        _louser = louser;
        _louser.Died += OnLouse;
    }

    private void OnLouse ()
    {
        _louser.Died -= OnLouse;
        LouseLevelMenu louseLevelMenu = _windowService.Open<LouseLevelMenu>() as LouseLevelMenu;
        louseLevelMenu.LevelFailed(_levelConfig, _levelTimeline.Stars);
    }
}
