using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

public class StatHolder
{
    public readonly int OwnerID;
    private readonly IArmor _armor;

    private Dictionary<StatType, Stat> _stats = new();
    private IEntityConfig _config;

    public StatHolder(int ownerID, IEntityConfig config, IArmor armor = null)
    {
        OwnerID = ownerID;
        _config = config;
        _armor = armor;
        InitStats(_config.Stats);
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
