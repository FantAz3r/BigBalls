using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(menuName = "Configs/PoolServiceConfig")]
    public class PoolServiceConfig : ScriptableObject
    {
        public int DefaultPoolSize = 1;
        public int InitialBallPoolSize = 20;
        public int InitialEnemyPoolSize = 5;
        public int InitialTilePoolSize = 2;
        public int IinitialLootPoolSize = 3;
        public int IinitialDamageTextPoolSize = 3;
    }
}
