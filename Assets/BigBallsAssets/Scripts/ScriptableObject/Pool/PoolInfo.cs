using System;
using UnityEngine;

namespace BigBalls.Configs
{
    [Serializable]
    public class PoolInfo
    {
        public MonoBehaviour Prefab;
        public int StartPoolCount = 4;
    }
}