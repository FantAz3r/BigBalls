using BigBalls.GameplayObjects;
using System.Collections.Generic;

public class EntityRepository : IEntityRepository
{
    private Dictionary<IEntity, StatHolder> _entities = new();

    public void Add(IEntity entity, StatHolder statHolder)
    {
        if (_entities.ContainsKey(entity))
            return;

        _entities.Add(entity, statHolder);
    }

    public void Remove(IEntity entity)
    {
        if (_entities.ContainsKey(entity))
        {
            _entities.Remove(entity);
        }
    }

    public StatHolder Get(IEntity entity)
    {
        _entities.TryGetValue(entity, out StatHolder statHolder);
        return statHolder;
    }
}