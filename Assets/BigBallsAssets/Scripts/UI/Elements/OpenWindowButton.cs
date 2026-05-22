using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.UI
{
    public class OpenWindowButton<T> : ButtonClickHandler where T : WindowBase
    {
        [SerializeField] private WindowBase _closeWindow;

        private IWindowService _windowService;

        [Inject]
        public void Construct(IWindowService windowService)
        {
            _windowService = windowService;
        }

        protected override void OnClick()
        {
            _closeWindow.Close();
            _windowService.Open<T>();
        }
    }
}
