using System;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IInputService
    {
        event Action<Vector2> MoveDirectionSeted;
        event Action<Vector2> RotateDirectionSeted;
        event Action Attack;

        Vector2 CursorOrigin { get;}

        event Action<Vector2> CameraOrbitSeted;
        event Action<float> CameraZoomSeted;
        IInputService GetSelf();
        void Start();

        void EnableCamera ();
        void DisableCamera ();
        void EnableUI ();
        void DisableUI ();
        void EnablePlayer ();
        void DisablePlayer ();
        void Dispose ();
        void EnableGameplayInput ();
    }
}