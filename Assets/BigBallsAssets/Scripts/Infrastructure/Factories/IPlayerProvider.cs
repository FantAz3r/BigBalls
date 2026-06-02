using BigBalls.GameplayObjects;

namespace BigBalls.Services
{
    public interface IPlayerProvider
    {
        Player Player { get; }
        void Set(Player player);
    }
}