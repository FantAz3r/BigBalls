using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;
using VContainer;

public class GameSpeedButton : ButtonClickHandler
{
    [SerializeField] private float _timescaleValue;
    private ITimeService _timeService;

    [Inject]
    public void Construct(ITimeService timeService)
    {
        _timeService = timeService;
    }

    protected override void OnClick ()
    {
        _timeService.SetTimescale(_timescaleValue);
    }
}
