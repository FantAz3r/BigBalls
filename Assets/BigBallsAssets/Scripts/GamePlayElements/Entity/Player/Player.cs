using BigBalls.Configs;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Player : MonoBehaviour, IEntity
    {
        [field: SerializeField] public PlayerConfig PlayerConfig { get; private set; }
        [field: SerializeField] public ResourceCollector ResourceCollector { get; private set; }
        [field: SerializeField] public SquashOnShot SquashOnShot { get; private set; }
        [field: SerializeField] public TiltOnShot TiltOnShot { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Transform WeaponSpawnPoint { get; private set; }
        [field: SerializeField] public Transform FirePoint { get; private set; }

        public DeathHandler<Player> DeathHandler { get; private set; }
        public int Id { get; private set; }
        public PlayerAnimator PlayerAnimator { get; private set; }
        public Transform Transform => transform;
        public EntityEventHandler EventHandler { get; private set; }

        private void Awake()
        {
            EventHandler = new EntityEventHandler();
        }

        private void OnDestroy()
        {
            DeathHandler?.Unsubscribe();
        }

        public void Construct(int id, DeathHandler<Player> playerDeathHandler)
        {
            Id = id;
            DeathHandler = playerDeathHandler;
        }

        public void SetFirePoint (Cannon cannon)
        {
            FirePoint = cannon.FirePoint;
        }
    }
}