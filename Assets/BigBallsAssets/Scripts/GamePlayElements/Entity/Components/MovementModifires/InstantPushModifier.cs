using UnityEngine;

public class InstantPushModifier : MovementModifier
{
    private Vector3 _pushForce;
    private bool _applied;

    public InstantPushModifier(Vector3 pushForce)
    {
        _pushForce = pushForce;
        _applied = false;
    }

    public override Vector3 Modify(Vector3 velocity)
    {
        if (_applied)
            return velocity;

        _applied = true;
        return velocity + _pushForce;
    }
}
