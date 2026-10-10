using System.Collections.Generic;
using BigBalls.Services;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

public class SoundSpawner
{
    private readonly IPoolService _poolService;
    private readonly ISoundService _soundService;
    private readonly SoundObject _soundObject;

    private readonly SoundData _soundData;
    private readonly Dictionary<AudioClip, float> _nextAllowedTime = new();

    private float _minDelayBetweenSameClip = 0.05f;
    private bool _canSpawn;

    [Inject]
    public SoundSpawner (IResourceLoader resourceLoader, IPoolService poolService, ISoundService soundService)
    {
        _soundObject = resourceLoader.Load<SoundObject>();
        _soundData = resourceLoader.Load<SoundData>();
        _soundService = soundService;
        _poolService = poolService;
    }

    public void Enable () => _canSpawn = true;
    public void Disable () => _canSpawn = false;

    public void Spawn (SoundType type)
    {
        Spawn(_soundData[type]);
    }

    public SoundObject Spawn (SoundStruct sound, bool isBackgroundSound = false)
    {
        if (!_canSpawn)
            return null;

        if (sound.Clip == null)
            return null;

        if (IsBlocked(sound.Clip))
            return null;

        AudioMixerGroup soundGroup = isBackgroundSound ? _soundData.MusicGroup : _soundData.EffectGroup;
        SoundObject soundObject = _poolService.Get(_soundObject);

        soundObject.Ended += OnClipEnded;
        soundObject.AudioSource.outputAudioMixerGroup = soundGroup;

        float volume = sound.Volume == 0 ? 1f : sound.Volume;
        float pitch = sound.Pitch == 0 ? 1f : sound.Pitch;

        soundObject.PlayAndDisable(sound.Clip, volume, pitch, sound.NeedRandomPitch);

        _soundService.Add(soundObject);
        Block(sound.Clip);
        return soundObject;
    }

    private bool IsBlocked (AudioClip clip)
    {
        return _nextAllowedTime.TryGetValue(clip, out float nextTime)
               && Time.unscaledTime < nextTime;
    }

    private void Block (AudioClip clip)
    {
        _nextAllowedTime[clip] = Time.unscaledTime + _minDelayBetweenSameClip;
    }

    private void OnClipEnded (SoundObject soundObject)
    {
        soundObject.Ended -= OnClipEnded;
        _poolService.Release(soundObject);
    }
}