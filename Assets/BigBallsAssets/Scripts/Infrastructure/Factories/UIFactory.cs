using UnityEngine;
using VContainer;
using VContainer.Unity;
//using YG;
using BigBalls.UI;
using BigBalls.Services;
using BigBalls.Configs;

namespace BigBalls.Factories
{
    public class UIFactory : IUIFactory
    {
        private readonly ITimeService _timeService;
        private readonly ILevelLoadingService _levelLoadingService;
        private readonly IObjectResolver _objectResolver;
        private readonly IResourceLoader _resourceLoader;

        private WindowData _windowData;
        private SafeAreaUIHolder _safeAreaUIHolder;
        private UIRoot _uiRoot;

        public HUD HUD { get; private set; }

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

        public void CreateUIRoot()
        {
            _uiRoot = _objectResolver.Instantiate(_resourceLoader.Load<UIRoot>());
            //_safeAreaUIHolder = _uiRoot.GetComponentInChildren<SafeArea>().transform;
        }

        public HUD CreateHUD()
        {
            if (HUD != null)
            {
                HUD.Open();
                return HUD;
            }

            HUD = CreateWindow(WindowType.HUD) as HUD;

            return HUD;
        }

        public SettingsView CreateSettings()
        {
            return CreateWindow(WindowType.MainSettings) as SettingsView;
        }

        public void CreateJoystick()
        {
            //if (YG2.envir.isDesktop == false)
            //{
            //
            //}
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
