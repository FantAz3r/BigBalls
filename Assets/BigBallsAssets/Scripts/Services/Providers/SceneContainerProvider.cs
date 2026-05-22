using BigBalls.GameplayObjects;
using BigBalls.Providers;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Services
{
    public class SceneContainerProvider : MonoBehaviour, ISceneContainerProvider
    {
        [field: SerializeField] public List<PlayerSpawnPoint> PlayerSpawnPoints { get; private set; }

    }
}
