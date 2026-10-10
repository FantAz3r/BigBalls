using BigBalls.Infrastructure;

namespace BigBalls.Services
{
    public interface ILevelLoadingService : IService
    {
        LevelID CurrentLevel { get; }

        void Load(LevelID level);
    }
}