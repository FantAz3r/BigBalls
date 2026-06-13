using BigBalls.GameplayObjects;
using System;
using UnityEngine;

namespace BigBalls.Configs
{
    [Serializable]
    public abstract class EffectConfig: ScriptableObject
    {
        public BehaviourType Type;
    }
}