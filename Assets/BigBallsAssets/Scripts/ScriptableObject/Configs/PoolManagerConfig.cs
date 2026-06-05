using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(menuName = "Configs/PoolManagerConfig")]
    public class PoolManagerConfig : ScriptableObject
    {
        public List<PoolInfo> Objects;
    }
}
