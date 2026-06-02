using BigBalls.GameplayObjects;

namespace BigBalls.Services
{
    public class PlayerProvider: IPlayerProvider
    {
        public Player Player { get; private set; }
        public void Set(Player player) => Player = player;
    }
}