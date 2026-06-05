using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(menuName = "Datas/BallsData")]

    public class BallsData : ScriptableObject
    {
        public List<BallConfig> BallConfigs = new List<BallConfig>();
    }
}