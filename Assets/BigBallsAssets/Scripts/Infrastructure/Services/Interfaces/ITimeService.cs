namespace BigBalls.Services
{
    public interface ITimeService : IService
    {
        bool IsPaused { get; }
        void StopGame();
        void ResumeGame();
        void SmoothRecoverTimescale(float duration);
        void SetTimescale (float value);
        void SmoothEditTimescale (float targetTimescale, float duration);
    }
}