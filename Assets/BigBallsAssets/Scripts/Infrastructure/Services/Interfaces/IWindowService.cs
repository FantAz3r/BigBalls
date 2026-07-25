using BigBalls.UI;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IWindowService : IService
    {
        WindowBase Open<T>(GameObject payload = null)
            where T : WindowBase;

        WindowBase OpenPreviousWindow ();
        void CreateUIRoot();
        void CreateJoystick();
    }
}
