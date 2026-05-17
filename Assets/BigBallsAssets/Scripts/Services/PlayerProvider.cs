using BigBalls.GameplayObjects;
using BigBalls.Providers;

namespace BigBalls.Services
{
    public class PlayerProvider : IPlayerProvider
    {
        private Player _player;
        public Player Player => _player;

        public void Set(Player player)
        {
            _player = player;
        }
    }
}
