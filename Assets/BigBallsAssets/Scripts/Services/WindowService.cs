using BigBalls.UI;
using UnityEngine;

namespace BigBalls.Services
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory _uiFactory;

        private WindowBase _currentWindow = null;
        private WindowType _currentWindowType = WindowType.None;
        private WindowType _previousWindowType = WindowType.None;

        public WindowService(IUIFactory uiFactory) => _uiFactory = uiFactory;

        public WindowBase Open(WindowType type, GameObject payload = null)
        {
            if (type == WindowType.None && type == _currentWindowType) 
                return null;

            _previousWindowType = _currentWindowType;
            _currentWindowType = type;

            switch (type)
            {
                case WindowType.None:
                    break;

                case WindowType.HUD:
                    _currentWindow = _uiFactory.CreateHUD();
                    break;

                case WindowType.Settings:
                    _currentWindow = _uiFactory.CreateSettings();
                    break;

                case WindowType.MainMenu:
                    _currentWindow = _uiFactory.CreateMainMenu();
                    break;

                case WindowType.LevelSelect:

                    _currentWindow = _uiFactory.CreateLevelSelect();
                    break;
            }

            if (_currentWindow != null)
            {
                _currentWindow.Open();
            }

            return _currentWindow;
        }

        public WindowBase OpenPreviousWindow()
        {
            if (_previousWindowType != WindowType.None)
            {
                var temp = _currentWindowType;
                _currentWindowType = _previousWindowType;
                _previousWindowType = temp;

                return Open(_currentWindowType);
            }
            else
            {
                return null;
            }
        }

        public void CreateUIRoot()
        {
            _uiFactory.CreateUIRoot();
        }

        public void CreateJoystick()
        {
            _uiFactory.CreateJoystick();
        }
    }
}
