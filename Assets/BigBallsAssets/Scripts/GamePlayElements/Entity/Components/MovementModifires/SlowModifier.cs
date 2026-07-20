using UnityEngine;

public class SlowModifier : MovementModifier
{
    private float _slowFactor;
    private float _duration;
    private float _elapsedTime;

    public SlowModifier(float slowFactor, float duration)
    {
        _slowFactor = Mathf.Clamp01(slowFactor);
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

        return velocity * (1f - _slowFactor);
    }
}