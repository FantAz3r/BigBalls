using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class EffectView : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<EffectType, ParticleObject> _effects = new SerializedDictionary<EffectType, ParticleObject>();

    public void EnableEffect(EffectType type, float duration = 0) => _effects[type].Play(duration);

}
