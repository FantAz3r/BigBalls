using BigBalls.UI;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IWindowService : IService
    {
        WindowBase Open(WindowType type, GameObject payload1 = null);
        WindowBase OpenPreviousWindow();
        void CreateUIRoot();
        void CreateJoystick();
    }
}
