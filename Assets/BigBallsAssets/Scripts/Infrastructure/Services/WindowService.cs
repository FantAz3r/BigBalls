using BigBalls.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Services
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory _uiFactory;
        private readonly Dictionary<Type, Func<WindowBase>> _actionMap;
        private WindowBase _currentWindow = null;
        private WindowBase _previousWindow = null;

        public WindowService(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;

            _actionMap = new Dictionary<Type, Func<WindowBase>>()
            {
                [typeof(HUD)] = uiFactory.CreateHUD,
                [typeof(SettingsView)] = uiFactory.CreateSettings,
                [typeof(MainMenu)] = uiFactory.CreateMainMenu,
                [typeof(LevelSelectionPanel)] = uiFactory.CreateLevelSelect,
                [typeof(PauseWindow)] = uiFactory.CreatePauseWindow,
                [typeof(LouseLevelMenu)] = uiFactory.CreateLouseMenu,
                [typeof(WinLevelMenu)] = uiFactory.CreateWinMenu,
                [typeof(BallTreeUI)] = uiFactory.CreateBallTree,
                [typeof(CardSelectionMenu)] = uiFactory.CreateCardMenu,
            };
        }

        public WindowBase Open<T>(GameObject payload = null)
            where T : WindowBase
        {
            return Get(typeof(T));
        }

        public WindowBase OpenPreviousWindow<T>()
            where T : WindowBase
        {
            var temp = _currentWindow;
            _currentWindow = _previousWindow;
            _previousWindow = temp;

            return Get(_currentWindow.GetType());
        }

        private WindowBase Get(Type type)
        {
            if(_currentWindow != null && type == _currentWindow.GetType())
                return null;

            _previousWindow = _currentWindow;
            _currentWindow = _actionMap[type].Invoke();

            if (_currentWindow != null)
            {
                _currentWindow.Open();
            }

            return _currentWindow;
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
