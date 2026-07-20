using BigBalls.GameplayObjects;

public interface IEntityRepository
{
    void Remove(IEntity entity);
    StatHolder GetStatHolder(IEntity entity);
    ComponentContainer GetContainer(IEntity entity);
    void Add(IEntity entity, StatHolder statHolder, ComponentContainer componentContainer);
}