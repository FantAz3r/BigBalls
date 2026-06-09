using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(menuName = "Configs/PoolServiceConfig")]
    public class PoolServiceConfig : ScriptableObject
    {
        public int InitialBallPoolSize = 20;
        public int InitialEnemyPoolSize = 5;
        public int InitialTilePoolSize = 2;
    }
}
