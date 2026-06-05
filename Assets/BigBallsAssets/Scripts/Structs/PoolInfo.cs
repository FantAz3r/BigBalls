using System;
using UnityEngine;

namespace BigBalls.Configs
{
    [Serializable]
    public struct PoolInfo
    {
        public MonoBehaviour Prefab;
        public int StartPoolCount;
    }
}