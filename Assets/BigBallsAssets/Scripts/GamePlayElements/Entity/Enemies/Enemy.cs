using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble
    {
        [field: SerializeField] public EntityTrigger EntityTrigger { get; private set; }
        public int Id { get; private set; }
        public DeathHandler<Enemy> DeathHandler { get; private set; }
        public Transform Transform => transform;

        public event Action<Enemy> Disabled;
        
        public void Construct(int id, DeathHandler<Enemy> deathHandler)
        {
            DeathHandler = deathHandler;
            Id = id;
        }
        
        private void OnDestroy()
        {
            DeathHandler?.Unsubscribe();
        }
    }
}