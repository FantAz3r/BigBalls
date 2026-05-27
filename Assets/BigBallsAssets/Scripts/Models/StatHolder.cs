using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using System.Linq;

public class StatHolder 
{
    public readonly int OwnerID;

    private Dictionary<StatType, Stat> _stats = new();
    private IEntityConfig _config;

    public StatHolder(int ownerID, IEntityConfig config)
    {
        OwnerID = ownerID;
        _config = config;
        InitStats(_config.Stats);
    }

    public Dictionary<StatType, Stat> Stats => _stats;

    private void InitStats(List<StatStruct> stats)
    {
        _stats = stats.ToDictionary(stat => stat.StatType, stat => new Stat(stat.StatType, stat.StartMaxValue, stat.MinValue, stat.StartCurrentValue));
    }

    public Stat this[StatType type] => _stats[type]; 
}
