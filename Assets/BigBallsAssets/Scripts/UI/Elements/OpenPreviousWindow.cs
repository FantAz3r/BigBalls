using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;
using VContainer;

public class OpenPreviousWindow : ButtonClickHandler
{
    [SerializeField] private WindowBase _closeWindow;

    private IWindowService _windowService;

    [Inject]
    public void Construct (IWindowService windowService)
    {
        _windowService = windowService;
    }

    protected override void OnClick ()
    {
        _closeWindow.Close();
        _windowService.OpenPreviousWindow();
    }
}
