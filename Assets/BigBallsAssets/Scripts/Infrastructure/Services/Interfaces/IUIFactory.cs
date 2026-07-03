using BigBalls.UI;

namespace BigBalls.Services
{
    public interface IUIFactory : IService
    {
        HUD CreateHUD();
        void ClearCache();
        void CreateJoystick();
        void CreateUIRoot();
        MainMenu CreateMainMenu();
        SettingsView CreateSettings();
        LevelSelectionPanel CreateLevelSelect();
        PauseWindow CreatePauseWindow();
        T Get<T>(WindowType type) where T : WindowBase;
        LouseLevelMenu CreateLouseMenu();
        WinLevelMenu CreateWinMenu();
        BallTreeUI CreateBallTree();
        CardInventory CreateInventoryMenu();
        CardSelectionMenu CreateCardMenu();
        Background CreateBackgroung ();
    }
}
