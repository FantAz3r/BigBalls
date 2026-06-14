using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

public class StatHolder
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
        InitStats(_config.Stats);
    }

    public EntityType EntityType { get; private set; }
    public void Set(IArmor armor = null) => _armor = armor;
    public void Set(IBuffer helmet = null)
    {
        if (_effectFactory == null)
            throw new ArgumentNullException(nameof(_effectFactory));

        _effectBehaviours = _effectFactory.Create(helmet.EffectConfigs);
    }

    public Dictionary<StatType, Stat> Stats => _stats;

    private void InitStats(List<StatStruct> stats)
    {
        foreach (var stat in stats)
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

    public Stat this[StatType type] => _stats[type];
}
