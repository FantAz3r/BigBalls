using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "Datas", menuName = "Datas/LevelData")]

    public class LevelData : ScriptableObject
    {
        public List<LevelConfig> LevelConfigs = new List<LevelConfig>();
    }
}
