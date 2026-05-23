using BigBalls.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "Datas", menuName = "Datas/LevelData")]

    public class LevelData : ScriptableObject
    {
        public List<LevelConfig> LevelConfigs = new List<LevelConfig>();

        public LevelConfig Get(LevelID level) => LevelConfigs.Where(c => c.Level == level).FirstOrDefault();
    }
}
