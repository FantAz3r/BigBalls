using System;

namespace BigBalls.GameplayObjects
{
    public class EntityEventHandler : IEntityEventHandler
    {
        public event Action Spawned;
        public event Action Hited;
        public event Action<int> HitedEntity;
        public event Action<IEntity> Died;
        public event Action<IEntity> Returned;
        public event Action<IEntity> Suisided;

        public void HitEntity (int id) => HitedEntity?.Invoke(id);
        public void Die (IEntity entity) => Died?.Invoke(entity);
        public void Return (IEntity entity) => Returned?.Invoke(entity);
        public void Spawn () => Spawned?.Invoke();
        public void Suiside (IEntity entity) => Suisided?.Invoke(entity);
        public void Hit() => Hited?.Invoke();
    }
}