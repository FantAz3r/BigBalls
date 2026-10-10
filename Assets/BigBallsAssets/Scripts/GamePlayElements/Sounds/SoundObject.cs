using System;
using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(AudioSource))]
public class SoundObject : MonoBehaviour
{
    private AudioClip _audioClip;
    private Coroutine _coroutine;
    private float _currentPlayTime;
    private ISoundService _soundService;
    private ICoroutineRunner _coroutineRunner;

    public event Action<SoundObject> Ended;
    [field: SerializeField] public AudioSource AudioSource { get; private set; }


    [Inject]
    public void Construct (ISoundService soundService, ICoroutineRunner coroutineRunner)
    {
        _soundService = soundService;
        _coroutineRunner = coroutineRunner;
    }

    public void PlayAndDisable (AudioClip clip, float volume, float pitch, bool isNeedRandomPitch = false)
    {
        _audioClip = clip;
        AudioSource.clip = _audioClip;
        _currentPlayTime = 0;
        AudioSource.Play();
        AudioSource.volume = volume;
        AudioSource.pitch = isNeedRandomPitch ? UnityEngine.Random.Range(pitch * 0.9f, pitch * 1.1f) : pitch;
        _coroutine = _coroutineRunner.StartCoroutine(DisableOnEnd());
    }

    private void OnDestroy ()
    {
        _soundService.Remove(this);
    }

    private IEnumerator DisableOnEnd ()
    {
        while (AudioSource.isPlaying)
        {
            yield return null;
        }

        RemoveSound();
    }

    public void StopSound ()
    {
        if (_coroutine != null)
        {
            _coroutineRunner.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        AudioSource.Pause();
    }

    public void ContinueSound ()
    {
        AudioSource.UnPause();
        _coroutine = _coroutineRunner.StartCoroutine(DisableOnEnd());
    }

    public void RemoveSound()
    {
        AudioSource.Stop();
        _soundService.Remove(this);
        Ended?.Invoke(this);
    }
}
