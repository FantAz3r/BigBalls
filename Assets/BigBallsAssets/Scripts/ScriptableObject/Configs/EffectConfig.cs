using BigBalls.GameplayObjects;
using CustomInspector;
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