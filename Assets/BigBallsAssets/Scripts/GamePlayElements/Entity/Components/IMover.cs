using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public interface IMover : ISubscribable
    {
        Vector2 Direction { get; }
        Transform MovableObject { get; }

        void AddConstantForce (Vector3 direction, float duration);
        void AddInstantPush (Vector3 pushDirection, float force);
        void AddPush (Vector3 pushDirection, float force, float duration = 0.5F);

        Vector3 GetCurrentVelocity();
        void SetDirection(Vector2 direction);
        void SetDirection (Vector3 direction);
        void SetMovableObject (Transform transform);
        void SetReycastInfo(Vector3[] directions, Vector3[] points);
        void SetTarget(Transform target);
    }
}