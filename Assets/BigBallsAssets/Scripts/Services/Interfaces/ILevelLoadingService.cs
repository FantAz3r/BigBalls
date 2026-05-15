using BigBalls.Infrastructure;

namespace BigBalls.Services
{
    public interface ILevelLoadingService : IService

    {
        void Load(LevelID level);
    }
}