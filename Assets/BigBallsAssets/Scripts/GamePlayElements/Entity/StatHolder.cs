using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;

public class StatHolder : IArtefactUser
{
    private readonly IEntity _owner;
    private readonly IEffectFactory _effectFactory;

    private List<EffectBehaviour> _effectBehaviours;
    private Dictionary<StatType, Stat> _stats = new();
    private IEntityConfig _config;
    private IArmor _armor;

    public StatHolder(IEntity entity, EntityType entityType, IEntityConfig config, IEffectFactory effectFactory = null)
    {
        EntityType = entityType;
        _owner = entity;
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

    public void AddEffects(ArtefactModel artefactModel)
    {
        _effectBehaviours.AddRange(_effectFactory.Create(artefactModel.ArtefactConfig.Effects, artefactModel.Level));
    }

    public bool TryGetStat(out Stat stat, StatType type)
    {
        stat = null;

        if (_stats.ContainsKey(type))
        {
            stat = _stats[type];
            return true;
        }

        return false;
    }
}