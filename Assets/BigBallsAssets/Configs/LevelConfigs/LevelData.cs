using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Datas", menuName = "Datas/LevelData")]

public class LevelData : ScriptableObject
{
    public List<LevelConfig> LevelConfigs = new List<LevelConfig>();
}
