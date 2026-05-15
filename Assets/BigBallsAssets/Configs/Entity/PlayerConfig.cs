using UnityEngine;
using UnityEngine.Rendering;
using BigBalls.GameplayObjects;

namespace BigBalls.StaticData
{
    public class PlayerConfig : ScriptableObject
    {
        public SerializedDictionary<StatType, float> Stats = new SerializedDictionary<StatType, float>();
    }
}
