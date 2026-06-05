namespace BigBalls.Services
{
    public interface IUpdateService
    {
        void Register(IUpdateble tickable);
        void Unregister(IUpdateble tickable);
        void SetPaused(bool isPaused);
        void Clear();
    }
}