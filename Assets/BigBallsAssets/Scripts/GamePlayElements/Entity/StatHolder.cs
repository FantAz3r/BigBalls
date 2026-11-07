using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

public class StatHolder : IArtefactUser
{
    private readonly IEntity _owner;
    private readonly IEffectFactory _effectFactory;

    private List<EffectBehaviour> _effectBehaviours = new();
    private Dictionary<StatType, Stat> _stats = new();
    private IEntityConfig _config;
    private ArmorModel _armor;

    public StatHolder (IEntity entity, EntityType entityType, IEntityConfig config, IEffectFactory effectFactory = null)
    {
        EntityType = entityType;
        _owner = entity;
        _config = config;
        _effectFactory = effectFactory;
    }

    public EntityType EntityType { get; private set; }

    public Dictionary<StatType, Stat> Stats => _stats;

    public Stat this[StatType type] => _stats[type];

    public void Set (ArmorModel item)
    {
        _armor = item;

        foreach (var stat in _stats)
        {
            if (_armor != null && _armor.Stats.ContainsKey(stat.Key))
            {
                float currentValue = stat.Value.StartCurrentValue;
                float maxValue = stat.Value.StartMaxValue;

                stat.Value.SetMaxValue(maxValue + _armor.ArmorConfig.GetMaxStatValue(_armor.Level, stat.Key));
                stat.Value.SetCurrentValue(currentValue + _armor.ArmorConfig.GetCurrentStatValue(_armor.Level, stat.Key));
            }
            else
            {
                stat.Value.ResetStat();
            }
        }
    }

    public void InitStats ()
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

            _stats[stat.StatType] = new Stat(stat.StatType, maxValue, stat.MinValue, currentValue);
        }
    }

    public void AddEffects (ArtefactModel artefactModel)
    {
        List<EffectBehaviour> behaviours = _effectFactory.Create(artefactModel.ArtefactConfig.Effects, artefactModel.Level);
        _effectBehaviours.AddRange(behaviours);

        foreach (EffectBehaviour behaviour in _effectBehaviours)
        {
            behaviour.Subscribe(_owner);
        }
    }

    public bool TryGetStat (out Stat stat, StatType type)
    {
        stat = null;

        if (_stats.ContainsKey(type))
        {
            stat = _stats[type];
            return true;
        }

        return false;
    }

    public void RemoveEffects (ArtefactModel artefactModel)
    {
        foreach (EffectBehaviour behaviour in _effectBehaviours)
        {
            behaviour.Subscribe(_owner);
        }
    }
}