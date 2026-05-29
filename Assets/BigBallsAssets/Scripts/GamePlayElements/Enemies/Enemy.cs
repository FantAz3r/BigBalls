using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Enemy : MonoBehaviour, IEntity, IHitble
    {
        public int Id { get; private set; }
        public event Action OnWallCollision;
        public void Construct(int id)
        {
            Id = id;
        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<FrontWall>(out _))
            {

            }
        }
    }
}