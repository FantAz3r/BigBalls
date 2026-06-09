using BigBalls.GameplayObjects;

public interface IEntityRepository
{
    void Add(IEntity entity, StatHolder statHolder);
    void Remove(IEntity entity);
    StatHolder Get(int id);
}