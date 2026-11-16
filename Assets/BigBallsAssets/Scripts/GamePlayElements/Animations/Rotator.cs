using UnityEngine;

public class Rotator : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [SerializeField] private float _degreesPerSecond = 180f;
    [SerializeField] private bool _useUnscaledTime = false;
    [SerializeField] private bool _rotateLocal = true;
    [SerializeField] private int _direction = 1;
    [SerializeField] private RotationAxis _axis = RotationAxis.Y;

    private void Update ()
    {
        float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        int dir = _direction == 0 ? 1 : _direction;
        float step = _degreesPerSecond * Mathf.Sign(dir) * dt;

        Vector3 axis = GetAxisVector(_axis);

        if (_rotateLocal)
            transform.Rotate(axis, step, Space.Self);
        else
            transform.Rotate(axis, step, Space.World);
    }

    private static Vector3 GetAxisVector (RotationAxis axis)
    {
        return axis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.up
        };
    }
}