using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using YG;
using BigBalls.Configs;
using BigBalls.Services;
using BigBalls.UI;
using Crystal;

namespace BigBalls.Factories
{
    public class UIFactory : IUIFactory
    {
        private readonly ITimeService _timeService;
        private readonly ILevelLoadingService _levelLoadingService;
        private readonly IObjectResolver _objectResolver;
        private readonly IResourceLoader _resourceLoader;
        private readonly Dictionary<WindowType, WindowBase> _windowCache = new Dictionary<WindowType, WindowBase>();

        private WindowData _windowData;
        private SafeArea _safeAreaUIHolder;
        private UIRoot _uiRoot;

        public UIFactory(ITimeService timeService,
            ILevelLoadingService levelLoadingService,
            IObjectResolver objectResolver,
            IResourceLoader resourceLoader)
        {
            _timeService = timeService;
            _levelLoadingService = levelLoadingService;
            _objectResolver = objectResolver;
            _resourceLoader = resourceLoader;
            _windowData = _resourceLoader.Load<WindowData>();
        }
        public HUD HUD { get; private set; }

        public void CreateUIRoot()
        {
            _uiRoot = _objectResolver.Instantiate(_resourceLoader.Load<UIRoot>());
            _safeAreaUIHolder = _uiRoot.GetComponentInChildren<SafeArea>();
        }

        public HUD CreateHUD()
        {
            if (HUD != null)
            {
                return HUD;
            }

            HUD = GetOrCreateWindow(WindowType.HUD) as HUD;
            return HUD;
        }

        public SettingsView CreateSettings() => GetOrCreateWindow(WindowType.MainSettings) as SettingsView;

        public MainMenu CreateMainMenu() => GetOrCreateWindow(WindowType.MainMenu) as MainMenu;

        public void CreateJoystick()
        {
            //if (YG2.envir.isDesktop == false)
            //{
            //
            //}
        }

        public void ClearCache()
        {
            _windowCache.Clear();
            HUD = null;
        }

        private WindowBase GetOrCreateWindow(WindowType windowType, Transform parent = null)
        {
            if (_windowCache.TryGetValue(windowType, out var cachedWindow))
            {
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
                window = _objectResolver.Instantiate(prefab, _safeAreaUIHolder.transform);
            else
                window = _objectResolver.Instantiate(prefab, parent);

            _objectResolver.Inject(window);
            return window;
        }
    }
}
