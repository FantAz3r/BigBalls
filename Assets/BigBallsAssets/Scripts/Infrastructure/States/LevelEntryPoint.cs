using BigBalls.Factories;
using VContainer.Unity;

namespace BigBalls.Infrastructure
{
    public class LevelEntryPoint : IStartable
    {
        private readonly IPlayerFactory _playerFactory;

        public LevelEntryPoint(IPlayerFactory playerFactory)
        {
            _playerFactory = playerFactory;
        }

        public void Start()
        {
            _playerFactory.Create();
        }
    }
}
