using BigBalls.GameplayObjects;
using System.Collections.Generic;

public class EntityRepository : IEntityRepository
{
    private Dictionary<int, StatHolder> _entities = new(); 

    public void Add(IEntity entity, StatHolder statHolder)
    {
        if (_entities.ContainsKey(entity.Id))
            return;

        _entities.Add(entity.Id, statHolder);
    }

    public StatHolder Get(int id)
    {
        _entities.TryGetValue(id, out StatHolder statHolder);
        return statHolder;
    }
}
