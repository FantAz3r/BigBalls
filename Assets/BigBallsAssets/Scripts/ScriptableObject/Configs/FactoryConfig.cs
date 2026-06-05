using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(menuName = "Configs/FactoryConfig")]
    public class FactoryConfig : ScriptableObject
    {
        public GlobalPrefabConfig GlobalPrefabConfig;
    }
}