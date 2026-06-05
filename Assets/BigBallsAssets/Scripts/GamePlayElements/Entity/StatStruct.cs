using BigBalls.GameplayObjects;
using System;

namespace BigBalls.StaticData
{
    [Serializable]
    public struct StatStruct
    {
        public StatType StatType;
        public float StartMaxValue;
        public float StartCurrentValue;
        public float MinValue;
    }
}