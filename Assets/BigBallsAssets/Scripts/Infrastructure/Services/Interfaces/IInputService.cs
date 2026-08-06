using System;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IInputService
    {
        event Action<Vector2> MoveDirectionSeted;
        event Action<Vector2> RotateDirectionSeted;
        event Action Attack;
        IInputService GetSelf();
        void Enable();
        void Disable();
    }
}