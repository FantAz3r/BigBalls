using BigBalls.Services;
using UnityEngine;
using VContainer;

public class ShowFPSSetting : SwitchHandler
{
    private IUIFactory _windowService;
    private FPSCounter _fpsCounter;
    private ISaveService _saveService;

    [Inject]
    public void Construct(IUIFactory windowService, ISaveService saveService)
    {
        _windowService = windowService;
        _fpsCounter = _windowService.UIRoot.FPSCounter;
        _saveService = saveService;
    }

    private void Start()
    {
        Switch.IsOn = _saveService.GameProgress.ShowFPS;
    }

    protected override void OnClick(bool isOn)
    {
        _saveService.GameProgress.ShowFPS = isOn;
        _fpsCounter.ShowFPS(isOn);
        _saveService.Save();
    }
}
