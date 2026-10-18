using UnityEngine;

public class HitGroup : MonoBehaviour
{
    [SerializeField] private Collider[] _colliders;

    public int GroupId { get; private set; }

    private void Awake ()
    {
        GroupId = GetInstanceID();

        if (_colliders == null || _colliders.Length == 0)
            _colliders = GetComponentsInChildren<Collider>(includeInactive: true);
    }

    public bool Owns (Collider col)
    {
        foreach (var c in _colliders)
            if (c == col) return true;
        return false;
    }
}