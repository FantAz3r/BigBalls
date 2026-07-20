using UnityEngine;

public class ConstantForceModifier : MovementModifier
{
    private Vector3 _force;
    private float _duration;
    private float _elapsedTime;

    public ConstantForceModifier(Vector3 force, float duration)
    {
        _force = force;
        _duration = duration;
        _elapsedTime = 0f;
        IsActive = true;
    }

    public override Vector3 Modify(Vector3 velocity)
    {
        if (IsActive == false)
            return velocity;

        _elapsedTime += Time.fixedDeltaTime;

        if (_elapsedTime >= _duration)
        {
            IsActive = false;
            return velocity;
        }

        return velocity + _force * Time.fixedDeltaTime;
    }
}