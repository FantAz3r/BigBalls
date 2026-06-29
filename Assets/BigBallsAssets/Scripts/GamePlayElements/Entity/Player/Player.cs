using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Player : MonoBehaviour, IEntity
    {
        [field: SerializeField] public PlayerConfig PlayerConfig { get; private set; }
        [field: SerializeField] public ResourceCollector ResourceCollector { get; private set; }

        public DeathHandler<Player> DeathHandler { get; private set; }
        public HealHandler HealHandler { get; private set; }
        public int Id { get; private set; }
        public PlayerAnimator PlayerAnimator { get; private set; }
        public Transform Transform => transform;
        public EntityEventHandler EventHandler { get; private set; }

        private void Awake ()
        {
            EventHandler = new EntityEventHandler();
        }

        private void OnDestroy()
        {
            DeathHandler?.Unsubscribe();
        }

        public void Construct(int id, DeathHandler<Player> playerDeathHandler, HealHandler healHandler)
        {
            Id = id;
            DeathHandler = playerDeathHandler;
            HealHandler = healHandler;
        }
    }
}