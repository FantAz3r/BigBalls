using UnityEngine;

public class PushModifier : MovementModifier
{
    private Vector3 _pushForce;
    private float _duration;
    private float _elapsedTime;

    public PushModifier (Vector3 pushForce, float duration)
    {
        _pushForce = pushForce;
        _duration = duration;
        _elapsedTime = 0f;
        IsActive = true;
    }

    public override Vector3 Modify(Vector3 inputVelocity)
    {
        if (!IsActive)
            return inputVelocity;

        _elapsedTime += Time.fixedDeltaTime;

        if (_elapsedTime >= _duration)
        {
            IsActive = false;
            return inputVelocity;
        }

        float t = 1f - (_elapsedTime / _duration);
        return inputVelocity + (_pushForce * t);
    }
}
