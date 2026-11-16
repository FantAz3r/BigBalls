using System;

namespace BigBalls.Services
{
    public interface ITimeService : IService
    {
        bool IsPaused { get; }
        event Action<bool> IsPausedChanged;
        void StopGame();
        void ResumeGame();
        void SmoothRecoverTimescale(float duration);
        void SetTimescale (float value);
        void SmoothEditTimescale (float targetTimescale, float duration);
    }
}