using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class EntityTrigger : MonoBehaviour
    {
        [SerializeField] private SphereCollider _sphereCollider;

        public event Action WallDetected;
        public event Action PlayerDetected;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<BackWall>(out _))
            {
                WallDetected?.Invoke();
                Debug.Log("WallDetected");

            }

            if (other.TryGetComponent<Player>(out _))
            {
                Debug.Log("PlayerDetected");
                PlayerDetected?.Invoke();
            }
        }
    }
}