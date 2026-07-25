using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble
    {
        [field: SerializeField] public EntityTrigger EntityTrigger { get; private set; }
        [field: SerializeField] public HitFlash HitFlash { get; private set; }
        [field: SerializeField] public EnemyFallAnimator EnemyFallAnimator { get; private set; }
        [field: SerializeField] public EnemyHitedAnimation EnemyHitedAnimation { get; private set; }

        public int Id { get; private set; }
        public DeathHandler<Enemy> DeathHandler { get; private set; }
        public Transform Transform => transform;

        public EntityEventHandler EventHandler { get; private set; }

        private void Awake ()
        {
            EventHandler = new EntityEventHandler();
        }

        public void Construct (int id, DeathHandler<Enemy> deathHandler)
        {
            DeathHandler = deathHandler;
            Id = id;
        }
    }
}