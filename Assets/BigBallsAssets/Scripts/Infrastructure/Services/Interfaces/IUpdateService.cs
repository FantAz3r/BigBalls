namespace BigBalls.Services
{
    public interface IUpdateService
    {
        void Register(IUpdateble tickable);
        void Unregister(IUpdateble tickable);
        void Clear();
        void RegisterFixed(IUpdateble tickable);
        void UnregisterFixed(IUpdateble tickable);
    }
}