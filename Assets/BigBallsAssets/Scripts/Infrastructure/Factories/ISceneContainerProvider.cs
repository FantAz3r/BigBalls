using BigBalls.GameplayObjects;
using System.Collections.Generic;

namespace BigBalls.Providers
{
    public interface ISceneContainerProvider
    {
        List<PlayerSpawnPoint> PlayerSpawnPoints { get; }
    }
}