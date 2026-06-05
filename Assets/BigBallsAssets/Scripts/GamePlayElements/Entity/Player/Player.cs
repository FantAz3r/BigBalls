using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Player : MonoBehaviour, IEntity
    {
        private DeathHandler _playerDeathHandler;

        [field: SerializeField] public PlayerConfig PlayerConfig { get; private set; }
        [field: SerializeField] public ResourceCollector ResourceCollector { get; private set; }

        public int Id { get; private set; }
        public PlayerAnimator PlayerAnimator { get; private set; }
        public Transform Transform => transform;

        public void Construct(int id, DeathHandler playerDeathHandler)
        {
            Id = id;
            _playerDeathHandler = playerDeathHandler;
            _playerDeathHandler.Subscribe();

        }

        private void OnEnable()
        {
            _playerDeathHandler?.Subscribe();
        }

        private void OnDisable()
        {
            _playerDeathHandler?.Unsubscribe();
        }
    }
}
