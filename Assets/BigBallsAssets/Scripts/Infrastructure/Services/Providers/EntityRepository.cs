using System.Collections.Generic;
using BigBalls.GameplayObjects;

public class EntityRepository : IEntityRepository
{
    private Dictionary<int, StatHolder> _entities = new();

    public void Add (IEntity entity, StatHolder statHolder)
    {
        if (_entities.ContainsKey(entity.Id))
            return;

        _entities.Add(entity.Id, statHolder);
    }

    public void Remove (IEntity entity)
    {
        if (_entities.ContainsKey(entity.Id))
        {
            _entities.Remove(entity.Id);
            _entities[entity.Id] = null;
        }
    }

    public StatHolder Get (int id)
    {
        _entities.TryGetValue(id, out StatHolder statHolder);
        return statHolder;
    }
}