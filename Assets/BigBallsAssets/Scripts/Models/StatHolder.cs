using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using System.Linq;

public class StatHolder 
{
    public readonly int OwnerID;

    private List<Stat> _stats = new();
    private IEntityConfig _config;

    public StatHolder(int ownerID, IEntityConfig config)
    {
        OwnerID = ownerID;
        _config = config;
        InitStats(_config.Stats);
    }

    public IEnumerable<Stat> Stats => _stats;

    private void InitStats(IEnumerable<StatStruct> stats)
    {
        foreach (var stat in stats)
        {
            _stats.Add(new Stat(stat.StatType, stat.MaxValue, stat.MinValue));
        }
    }

    public Stat Get(StatType statType) => _stats.Where(stat => stat.Type == statType).FirstOrDefault();
}
