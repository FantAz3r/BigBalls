using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatHolder : IArtefactUser
{
    public readonly int OwnerID;
    private readonly IEffectFactory _effectFactory;

    private List<EffectBehaviour> _effectBehaviours;
    private Dictionary<StatType, Stat> _stats = new();
    private IEntityConfig _config;
    private IArmor _armor;

    public StatHolder(int ownerID, EntityType entityType, IEntityConfig config, IEffectFactory effectFactory = null)
    {
        EntityType = entityType;
        OwnerID = ownerID;
        _config = config;
        _effectFactory = effectFactory;
    }

    public EntityType EntityType { get; private set; }

    public Dictionary<StatType, Stat> Stats => _stats;

    public Stat this[StatType type] => _stats[type];

    public void Set(ArmorModel item)
    {
        _armor = item;
    }

    public void InitStats()
    {
        foreach (var stat in _config.Stats)
        {
            float maxValue = stat.StartMaxValue;
            float currentValue = stat.StartCurrentValue;

            if (_armor != null && _armor.Stats.ContainsKey(stat.StatType))
            {
                StatStruct statStruct = _armor.Stats[stat.StatType];
                maxValue += statStruct.StartMaxValue;
                currentValue += statStruct.StartCurrentValue;
            }

            _stats.Add(stat.StatType, new Stat(stat.StatType, maxValue, stat.MinValue, currentValue));
        }
    }

    public void AddEffects(List<EffectBehaviour> effects)
    {
        throw new NotImplementedException();
    }
}