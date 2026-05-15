using System;
using System.Numerics;

namespace BigBalls.Services
{
    public interface IInputService
    {
        event Action<Vector2> MovePerformed;
        event Action<Vector2> RotatePerformed;

        IInputService GetSelf();
        void EnableInput();
        void DisableInput();
    }
}