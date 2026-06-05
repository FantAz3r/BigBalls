using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble
    {
        private DeathHandler _deathHandler;

        [field: SerializeField] public EntityTrigger EntityTrigger { get; private set; }
        public int Id { get; private set; }
        public Transform Transform => transform;

        public void Construct(int id, DeathHandler deathHandler)
        {
            Id = id;
            _deathHandler = deathHandler;
            _deathHandler.Subscribe();
        }

        private void OnEnable()
        {
            _deathHandler?.Subscribe();
        }

        private void OnDisable()
        {
            _deathHandler?.Unsubscribe();
        }
    }
}