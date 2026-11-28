using System;
using System.Collections;
using BigBalls.GameplayObjects;
using UnityEngine;
using VContainer;

public class IceSegment : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _startScale = 1f;
    [SerializeField] private float _slowDuration = 0.7f;
    [SerializeField] private float _slowPower = 0.4f;

    private float _timer = 0f;
    private Coroutine _coroutine;
    private DebuffService _debuffService;

    public event Action<IceSegment> Returned;

    [Inject]
    public void Construct (DebuffService debuffService)
    {
        _debuffService = debuffService;
    }

    public void StartScale (float lifeTime)
    {
        _lifeTime = lifeTime;
        _coroutine = StartCoroutine(ScaleRoutine());
    }

    private IEnumerator ScaleRoutine ()
    {
        float currentScale = _startScale;

        while (currentScale > 0f)
        {
            currentScale = Mathf.MoveTowards(currentScale, 0f, _startScale / _lifeTime * Time.deltaTime);
            transform.localScale = Vector3.one * currentScale;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Returned?.Invoke(this);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            _debuffService.Apply(
                EffectType.Freeze,
                player,
                _slowDuration,
                _slowPower);
        }
    }
}
