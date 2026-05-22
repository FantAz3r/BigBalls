using BigBalls.GameplayObjects;
using System;

namespace BigBalls.StaticData
{
    [Serializable]
    public struct StatStruct
    {
        public StatType StatType;
        public float MaxValue;
        public float MinValue;
    }
}