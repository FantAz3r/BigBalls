using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private BoxCollider _boxCollider;

        private bool _isActionComplite = false;

        public event Action Finished;

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<Player>(out _))
            {
                if (_isActionComplite == false)
                {
                    Finished?.Invoke();
                    _isActionComplite = true;
                }
            }
        }
    }
}