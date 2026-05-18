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
    }
}
