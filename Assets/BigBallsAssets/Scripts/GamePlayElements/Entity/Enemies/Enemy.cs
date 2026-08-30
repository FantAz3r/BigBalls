using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble
    {
        [field: SerializeField] public EntityTrigger EntityTrigger { get; private set; }
        [field: SerializeField] public EntityCollision EntityCollision { get; private set; }
        [field: SerializeField] public ColorChanger ColorChanger { get; private set; }
        [field: SerializeField] public EnemyFallAnimation EnemyFallAnimator { get; private set; }
        [field: SerializeField] public EnemyHitedAnimation EnemyHitedAnimation { get; private set; }
        [field: SerializeField] public EffectView EffectViewer { get; private set; }
        [field: SerializeField] public EnemyIdleBob EnemyIdleBob { get; private set; }
        [field: SerializeField] public EnemyAnimator EnemyAnimator { get; private set; }
        [field: SerializeField] public Transform RotationPart { get; private set; }

        public int Id { get; private set; }
        public DeathHandler<Enemy> DeathHandler { get; private set; }
        public EnemyConfig Config { get; private set; }
        public StatHolder StatHolder { get; private set; }
        public EntityEventHandler EventHandler { get; private set; }
        public Transform Transform => transform;


        private void Awake ()
        {
            EventHandler = new EntityEventHandler();
        }

        public void Init (int id, DeathHandler<Enemy> deathHandler, EnemyConfig enemyConfig, StatHolder statHolder)
        {
            Config = enemyConfig;
            DeathHandler = deathHandler;
            Id = id;
            StatHolder = statHolder;
            ChildInit();
        }

        protected virtual void ChildInit()
        {

        }
    }
}