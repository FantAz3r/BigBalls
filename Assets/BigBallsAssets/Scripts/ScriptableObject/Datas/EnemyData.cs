using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Datas/EnemyData")]

    public class EnemyData : ScriptableObject
    {
        [ SerializeField] public List<EnemyConfig> Configs = new List<EnemyConfig>();
    }
}