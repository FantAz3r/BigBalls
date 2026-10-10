using BigBalls.GameplayObjects;
using System.Collections.Generic;

namespace BigBalls.Providers
{
    public interface ISceneContainerProvider
    {
        BackgroundLooper BackgroundLooper { get; }
        List<PlayerSpawnPoint> PlayerSpawnPoints { get; }
    }
}