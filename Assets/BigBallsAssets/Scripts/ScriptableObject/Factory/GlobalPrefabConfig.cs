using BigBalls.Infrastructure;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(menuName = "Configs/GlobalPrefabConfig")]
    public class GlobalPrefabConfig : ScriptableObject
    {
        public LevelID StartLevel = LevelID.Level1;
        public List<MonoBehaviour> Prefabs;
    }
}