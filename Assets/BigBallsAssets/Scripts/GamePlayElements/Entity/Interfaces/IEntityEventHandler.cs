using System;

namespace BigBalls.GameplayObjects
{
    public interface IEntityEventHandler
    {
        event Action<IEntity> Spawned;
        event Action<IEntity> HitedEntity;
        event Action<IEntity> Died;
        event Action<IEntity> Returned;
    }
}