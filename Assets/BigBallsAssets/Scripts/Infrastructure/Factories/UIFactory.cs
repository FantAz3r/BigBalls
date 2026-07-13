using BigBalls.Configs;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
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

        public UIFactory(
            IObjectResolverProvider resolverProvider,
            IResourceLoader resourceLoader)
        {
            _resolverProvider = resolverProvider;
            _resourceLoader = resourceLoader;
            _windowData = _resourceLoader.Load<WindowData>();
        }

        public UIRoot UIRoot { get; private set; }

        public T Get<T>(WindowType type) where T : WindowBase
        {
            if (_windowCache.ContainsKey(type) == false)
            {
                WindowBase window = GetOrCreateWindow(type);
                window.Close();
                return (T)window;
            }

            return (T)_windowCache[type];
        }

        public void CreateUIRoot()
        {
            UIRoot = _resolverProvider.CurrentResolver.Instantiate(_resourceLoader.Load<UIRoot>());
        }

        public HUD CreateHUD() => GetOrCreateWindow(WindowType.HUD) as HUD;

        public SettingsView CreateSettings() => GetOrCreateWindow(WindowType.Settings) as SettingsView;

        public MainMenu CreateMainMenu() => GetOrCreateWindow(WindowType.MainMenu) as MainMenu;

        public PauseWindow CreatePauseWindow() => GetOrCreateWindow(WindowType.Pause) as PauseWindow;

        public LouseLevelMenu CreateLouseMenu() => GetOrCreateWindow(WindowType.LouseLevelMenu) as LouseLevelMenu;

        public WinLevelMenu CreateWinMenu() => GetOrCreateWindow(WindowType.WinLevelMenu) as WinLevelMenu;

        public BallTreeUI CreateBallTree() => GetOrCreateWindow(WindowType.BallTree) as BallTreeUI;

        public CardSelectionMenu CreateCardMenu() => GetOrCreateWindow(WindowType.CardMenu) as CardSelectionMenu;

        public CardInventory CreateInventoryMenu() => GetOrCreateWindow(WindowType.Inventory) as CardInventory;

        public Background CreateBackgroung() => GetOrCreateWindow(WindowType.Background, UIRoot.BackgroundUIHolder) as Background;

        public ChestItemDropView CreateChestWindow() => GetOrCreateWindow(WindowType.OpenChest, UIRoot.BackgroundUIHolder) as ChestItemDropView;

        public void CreateJoystick()

        {
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
                window = _resolverProvider.CurrentResolver.Instantiate(prefab, UIRoot.SafeAreaUIHolder);
            else
                window = _resolverProvider.CurrentResolver.Instantiate(prefab, parent);

            return window;
        }
    }
}
