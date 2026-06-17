using BigBalls.GameplayObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [Serializable]
    public abstract class EffectConfig : ScriptableObject
    {
        public List<EntityType> OwnersType;
        public abstract BehaviourType Type { get; }
    }
}