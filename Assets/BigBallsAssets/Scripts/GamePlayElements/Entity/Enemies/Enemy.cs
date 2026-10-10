using AYellowpaper;
using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble
    {
        [field: SerializeField] public EntityTrigger EntityTrigger { get; private set; }
        [field: SerializeField] public EntityCollision EntityCollision { get; private set; }
        [field: SerializeField] public ColorChanger ColorChanger { get; private set; }
        [field: SerializeField] public InterfaceReference<ISpawnAnimation> EnemySpawnAnimation { get; private set; }
        //[SerializeField] private InterfaceReference<ISpawnAnimation> EnemySpawnAnimation1;
        [field: SerializeField] public EnemyHitedAnimation EnemyHitedAnimation { get; private set; }
        [field: SerializeField] public EffectViewer EffectViewer { get; private set; }
        [field: SerializeField] public EnemyIdleBob EnemyIdleBob { get; private set; }
        [field: SerializeField] public EnemyAnimator EnemyAnimator { get; private set; }
        [field: SerializeField] public Transform RotationPart { get; private set; }
        [field: SerializeField] public DieAnimation DieAnimation { get; private set; }
        [field: SerializeField] public ShieldView ShieldView { get; private set; }

        public int Id { get; private set; }
        public IMover Mover { get; private set; }
        public DeathHandler<Enemy> DeathHandler { get; private set; }
        public EnemyConfig Config { get; private set; }
        public StatHolder StatHolder { get; private set; }
        public EntityEventHandler EventHandler { get; private set; }
        public Transform Transform => transform;
        public Rotator Rotator { get; private set; }
        public EnemyShooter Shooter { get; private set; }
        public Vector3 LocalScale { get; private set; }

        private void Awake ()
        {
            LocalScale = transform.localScale;
            EventHandler = new EntityEventHandler();
        }

        public void Init (
            int id,
            DeathHandler<Enemy> deathHandler,
            EnemyConfig enemyConfig,
            StatHolder statHolder,
            Rotator rotator,
            IMover mover,
            EnemyShooter shooter)
        {
            Shooter = shooter;
            Mover = mover;
            Config = enemyConfig;
            DeathHandler = deathHandler;
            Id = id;
            StatHolder = statHolder;
            Rotator = rotator;
            ChildInit();
        }

        protected virtual void ChildInit ()
        {
        }
    }
}