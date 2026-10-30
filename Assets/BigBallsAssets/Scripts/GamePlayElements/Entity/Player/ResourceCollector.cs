using System.Collections;
using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    [SerializeField] private SphereCollider _sphereCollider;

    private float _moveSpeed = 9f;
    private float _arrivalThreshold = 0.3f;
    private Stat _stat;

    public float SrartRange { get; private set; }
    public float CurrentRange => _sphereCollider.radius;

    private void OnTriggerEnter (Collider other)
    {
        if (other.TryGetComponent(out Loot loot))
        {
            StartCoroutine(RelocateCoroutine(loot));
        }
    }

    private void OnDestroy ()
    {
        if (_stat != null)
            _stat.ValueChanged -= SetRange;
    }

    public void Init (Stat range)
    {
        _stat = range;
        _stat.ValueChanged += SetRange;
        SetRange(range);
    }

    public void SetRange(IReadonlyStat readonlyStat)
    {
        _sphereCollider.radius = readonlyStat.CurrentValue;
    }

    private IEnumerator RelocateCoroutine (Loot loot)
    {
        loot.Collider.enabled = false;

        while (Vector3.SqrMagnitude(loot.transform.position - transform.position) > _arrivalThreshold * _arrivalThreshold)
        {
            loot.transform.position = Vector3.Lerp(
                loot.transform.position,
                transform.position,
                _moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        loot.transform.position = transform.position;
        loot.Collect();
        loot.Collider.enabled = true;
    }
}
