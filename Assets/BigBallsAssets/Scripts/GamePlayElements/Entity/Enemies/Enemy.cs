using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble, IDieble
    {
        [field: SerializeField] public EntityTrigger EntityTrigger { get; private set; }
        public int Id { get; private set; }
        public Transform Transform => transform;

        public void Construct(int id)
        {
            Id = id;
        }

        public IDieble GetSelf() => this;
    }
}