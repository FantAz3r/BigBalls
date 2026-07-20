using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public interface IMover
    {
        Vector2 Direction { get; }
        Transform MovableObject { get; }

        void AddConstantForce(Vector3 force, float duration);
        void AddInstantPush(Vector3 pushForce);
        void AddPush(Vector3 pushForce, float duration = 0.5F);
        Vector3 GetCurrentVelocity();
        void SetDirection(Vector2 direction);
        void SetReycastInfo(Vector3[] directions, Vector3[] points);
        void SetTarget(Transform target);
    }
}