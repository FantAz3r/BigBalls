namespace BigBalls.Services
{
    public interface ITimeService : IService
    {
        bool IsPaused { get; }
        void StopGame();
        void ResumeGame();
        void SmoothRecoverTimescale(float duration);
        void SetTimescale (float value);
        void SmoothReduceTimescale (float targetTimescale, float duration);
    }
}