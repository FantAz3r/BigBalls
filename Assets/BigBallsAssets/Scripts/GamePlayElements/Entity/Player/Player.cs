using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Player : MonoBehaviour, IEntity
    {
        private DeathHandler<Player> _playerDeathHandler;

        [field: SerializeField] public PlayerConfig PlayerConfig { get; private set; }
        [field: SerializeField] public ResourceCollector ResourceCollector { get; private set; }

        public int Id { get; private set; }
        public PlayerAnimator PlayerAnimator { get; private set; }
        public Transform Transform => transform;

        public void Construct(int id )
        {
            Id = id;
        }
    }
}
