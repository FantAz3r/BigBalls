using BigBalls.GameplayObjects;

namespace BigBalls.Providers
{
    public interface IPlayerProvider
    {
        Player Player { get; }
        void Set(Player player);
    }
}