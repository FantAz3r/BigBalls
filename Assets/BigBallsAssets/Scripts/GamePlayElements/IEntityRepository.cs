using BigBalls.GameplayObjects;

public interface IEntityRepository
{
    void Add(IEntity entity, StatHolder statHolder);
    StatHolder Get(int id);
}