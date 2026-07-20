using BigBalls.GameplayObjects;
using System.Collections.Generic;

public class EntityRepository : IEntityRepository
{
    private Dictionary<IEntity, (StatHolder StatHolder, ComponentContainer Container)> _entities = new();

    public void Add(IEntity entity, StatHolder statHolder, ComponentContainer componentContainer)
    {
        if (_entities.ContainsKey(entity))
            return;

        _entities.Add(entity, (statHolder, componentContainer));
    }

    public void Remove(IEntity entity)
    {
        if (_entities.ContainsKey(entity))
        {
            _entities.Remove(entity);
        }
    }

    public StatHolder GetStatHolder(IEntity entity)
    {
        _entities.TryGetValue(entity, out (StatHolder StatHolder, ComponentContainer Container) statHolder);
        return statHolder.StatHolder;
    }

    public ComponentContainer GetContainer(IEntity entity)
    {
        _entities.TryGetValue(entity, out (StatHolder StatHolder, ComponentContainer Container) statHolder);
        return statHolder.Container;
    }
}