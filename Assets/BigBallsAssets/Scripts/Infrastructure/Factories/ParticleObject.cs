using System;
using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class ParticleObject : MonoBehaviour
{
    private ICoroutineRunner _coroutineRunner;
    private Coroutine _watcherCoroutine;
    [field: SerializeField] public ParticleSystem ParticleSystem { get; private set; }

    public event Action<ParticleObject> Returned;

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void Play (float duration = 0)
    {
        if (ParticleSystem == null)
            return;

        ParticleSystem.Clear();

        gameObject.SetActive(true);
        ParticleSystem.Play();
        _watcherCoroutine = _coroutineRunner.StartCoroutine(WatchForEnd(duration));
    }

    public void Stop ()
    {
        if (ParticleSystem == null)
            return;

        if (_watcherCoroutine != null)
        {
            _coroutineRunner.StopCoroutine(_watcherCoroutine);
            _watcherCoroutine = null;
        }

        Returned?.Invoke(this);
        ParticleSystem.Stop();
        gameObject.SetActive(false);
    }

    private IEnumerator WatchForEnd (float duration = 0)
    {
        if (duration == 0)
        {
            while (ParticleSystem != null && ParticleSystem.IsAlive(true))
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        _watcherCoroutine = null;
        Stop();
    }

    private void OnDisable ()
    {
        Stop();
    }
}