using System.Collections.Generic;
using BigBalls.Infrastructure;
using BigBalls.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public class GameTutorial : PopupWindow, IPointerClickHandler
{
    [SerializeField] private List<RectTransform> _panels = new();

    private ITimeService _timeService;
    private CreateLevelState _createLevelState;
    private int _panelID;

    [Inject]
    public void Construct (ITimeService timeService, CreateLevelState createLevelState)
    {
        _timeService = timeService;
        _createLevelState = createLevelState;
    }

    public override void Open ()
    {
        if (_timeService == null)
            return;

        if (_createLevelState.LevelConfig.Level != LevelID.Level0)
            return;

        base.Open();
        _timeService.StopGame();

        foreach (var panel in _panels)
            panel.Disable();

        SwapPanel();
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        SwapPanel();
    }

    private void SwapPanel ()
    {
        if (_panelID > 0)
            _panels[_panelID - 1].Disable();

        if (_panelID == _panels.Count)
        {
            Close();
            _timeService.ResumeGame();
            return;
        }

        _panels[_panelID].Enable();
        _panelID++;
    }
}
