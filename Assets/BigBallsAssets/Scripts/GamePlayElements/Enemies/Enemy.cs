using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble
    {
        public int Id { get; private set; }

        public void Construct(int id)
        {
            Id = id;
        }
    }
}