using BigBalls.GameplayObjects;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "BallConfig", menuName = "Configs/BallConfig")]

    public class BallConfig : ScriptableObject
    {
        [field: SerializeField] public Ball Prefab { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public List<BehaviourType> BehaviourTypes { get; private set; }
    }
}