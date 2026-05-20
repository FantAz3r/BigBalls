using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.UI
{
    public class OpenWindowButton : ButtonClickHandler
    {
        [SerializeField] private WindowBase _closeWindow;
        [SerializeField] private WindowType _openWindow;

        private IWindowService _windowService;

        [Inject]
        public void Construct(IWindowService windowService)
        {
            _windowService = windowService;
        }

        protected override void OnClick()
        {
            _closeWindow.Close();

            if (_openWindow == WindowType.PreviousWindow)
            {
                _windowService.OpenPreviousWindow();
            }
            else
            {
                _windowService.Open(_openWindow);
            }
        }
    }
}
