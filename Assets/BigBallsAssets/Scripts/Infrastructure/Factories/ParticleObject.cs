using System;
using System.Collections;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    [field: SerializeField] public ParticleSystem ParticleSystem { get; private set; }

    public event Action<ParticleObject> Returned;

    private bool _isReturning;
    private Coroutine _watcherCoroutine;

    private void Awake ()
    {
        if (ParticleSystem == null)
            ParticleSystem = GetComponentInChildren<ParticleSystem>(true);
    }

    public void Init (ParticleSystem particleSystem)
    {
        ParticleSystem = Instantiate(particleSystem, transform);
    }

    public void Play ()
    {
        _isReturning = false;

        if (_watcherCoroutine != null)
        {
            StopCoroutine(_watcherCoroutine);
            _watcherCoroutine = null;
        }

        ParticleSystem.Clear();
        ParticleSystem.Play();
        _watcherCoroutine = StartCoroutine(WatchForEnd());
    }

    public void Stop()
    {
        ParticleSystem.Stop();
        OnParticleEndedInternal();
    }

    private void OnParticleEndedInternal ()
    {
        if (_isReturning)
            return;

        _isReturning = true;
        Destroy(ParticleSystem.gameObject);
        Returned?.Invoke(this);
    }

    private IEnumerator WatchForEnd ()
    {
        while (ParticleSystem != null && ParticleSystem.IsAlive(true))
        {
            yield return null;
        }

        _watcherCoroutine = null;

        if (_isReturning)
            yield break;

        OnParticleEndedInternal();
    }

    private void OnDestroy ()
    {
        Returned = null;

        if (_watcherCoroutine != null)
        {
            StopCoroutine(_watcherCoroutine);
            _watcherCoroutine = null;
        }
    }
}