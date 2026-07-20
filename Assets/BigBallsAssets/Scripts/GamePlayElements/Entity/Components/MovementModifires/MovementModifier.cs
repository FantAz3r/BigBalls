using UnityEngine;

public abstract class MovementModifier
{
    public bool IsActive { get; protected set; }
    public abstract Vector3 Modify(Vector3 velocity);
}
