using System;

namespace BigBalls.GameplayObjects
{
    public interface IEntityEventHandler
    {
        event Action Spawned;
        event Action<int> HitedEntity;
        event Action<IEntity> Died;
        event Action<IEntity> Returned;
    }
}