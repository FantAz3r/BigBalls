using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using BigBalls.Configs;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(menuName = "Datas/BallsData")]

    public class BallsData : ScriptableObject
    {
        public SerializedDictionary<BallType, BallConfig> BallConfigs = new();
        public List<BallConfig> BallConfigsList => BallConfigs.Values.ToList();
    }
}