using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public interface IMover
    {
        Vector2 Direction { get; }

        void SetDirection (Vector2 direction);
        void SetReycastInfo (Vector3[] directions, Vector3[] points);
        void SetTarget (Transform target);
    }
}