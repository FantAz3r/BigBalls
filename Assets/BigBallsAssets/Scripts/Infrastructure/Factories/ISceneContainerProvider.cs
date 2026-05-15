using UnityEngine;

namespace BigBalls.Providers
{
    public interface ISceneContainerProvider
    {
        Transform PlayerSpawnPoint { get; }
    }
}