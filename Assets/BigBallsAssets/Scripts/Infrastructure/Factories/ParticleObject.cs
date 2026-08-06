using System;
using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class ParticleObject : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private ICoroutineRunner _coroutineRunner;
    private Coroutine _watcherCoroutine;

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void Play (float duration = 0)
    {
        if (_particleSystem == null)
            return;

        Stop();
        _particleSystem.Clear();

        gameObject.SetActive(true);
        _particleSystem.Play();
        _watcherCoroutine = _coroutineRunner.StartCoroutine(WatchForEnd(duration));
    }

    public void Stop ()
    {
        if (_particleSystem == null)
            return;

        if (_watcherCoroutine != null)
        {
            _coroutineRunner.StopCoroutine(_watcherCoroutine);
            _watcherCoroutine = null;
        }

        _particleSystem.Stop();
        gameObject.SetActive(false);
    }

    private IEnumerator WatchForEnd (float duration = 0)
    {
        if (duration == 0)
        {
            while (_particleSystem != null && _particleSystem.IsAlive(true))
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