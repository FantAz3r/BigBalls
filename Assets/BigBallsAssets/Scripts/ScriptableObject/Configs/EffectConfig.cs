using BigBalls.GameplayObjects;
using System;
using UnityEngine;

namespace BigBalls.StaticData
{
    [Serializable]
    public abstract class EffectConfig: ScriptableObject
    {
        public BehaviourType Type;
    }
}