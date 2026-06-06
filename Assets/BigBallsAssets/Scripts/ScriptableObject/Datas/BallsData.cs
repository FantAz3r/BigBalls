using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(menuName = "Datas/BallsData")]

    public class BallsData : ScriptableObject
    {
        public SerializedDictionary<BallType, BallConfig> BallConfigs = new();
    }
}