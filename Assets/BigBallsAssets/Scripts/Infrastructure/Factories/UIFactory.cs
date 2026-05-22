using BigBalls.Configs;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
using Crystal;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class UIFactory : IUIFactory
    {
        private readonly IObjectResolverProvider _resolverProvider;
        private readonly IResourceLoader _resourceLoader;
        private readonly Dictionary<WindowType, WindowBase> _windowCache = new Dictionary<WindowType, WindowBase>();

        private WindowData _windowData;
        private SafeArea _safeAreaUIHolder;
        private UIRoot _uiRoot;

        public UIFactory(
            IObjectResolverProvider resolverProvider,
            IResourceLoader resourceLoader)
        {
            _resolverProvider = resolverProvider;
            _resourceLoader = resourceLoader;
            _windowData = _resourceLoader.Load<WindowData>();
        }

        public void CreateUIRoot()
        {
            _uiRoot = Object.Instantiate(_resourceLoader.Load<UIRoot>());
            _safeAreaUIHolder = _uiRoot.GetComponentInChildren<SafeArea>();
        }

        public HUD CreateHUD() => GetOrCreateWindow(WindowType.HUD) as HUD;

        public SettingsView CreateSettings() => GetOrCreateWindow(WindowType.Settings) as SettingsView;

        public MainMenu CreateMainMenu() => GetOrCreateWindow(WindowType.MainMenu) as MainMenu;

        public PauseWindow CreatePauseWindow()=> GetOrCreateWindow(WindowType.Pause) as PauseWindow;

        public void CreateJoystick()
        {
            //if (YG2.envir.isDesktop == false)
            //{
            //
            //}
        }

        public LevelSelectionPanel CreateLevelSelect() => GetOrCreateWindow(WindowType.LevelSelect) as LevelSelectionPanel;

        public void ClearCache()
        {
            _windowCache.Clear();
        }

        private WindowBase GetOrCreateWindow(WindowType windowType, Transform parent = null)
        {
            if (_windowCache.TryGetValue(windowType, out var cachedWindow))
            {
                Debug.Log("from cach");
                cachedWindow.Open();
                return cachedWindow;
            }

            var newWindow = CreateWindow(windowType, parent);
            _windowCache.Add(windowType, newWindow);
            newWindow.Open();
            return newWindow;
        }

        private WindowBase CreateWindow(WindowType windowType, Transform parent = null)
        {
            WindowBase window;
            WindowBase prefab = _windowData.Get(windowType);

            if (parent == null)
                window = _resolverProvider.CurrentResolver.Instantiate(prefab, _safeAreaUIHolder.transform);
            else
                window = _resolverProvider.CurrentResolver.Instantiate(prefab, parent);

            _resolverProvider.CurrentResolver.Inject(window);
            return window;
        }
    }
}
