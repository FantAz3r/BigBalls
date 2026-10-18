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

    public SoundObject Spawn (SoundType type)
    {
        return Spawn(_soundData[type]);
    }

    public SoundObject Spawn (SoundType type, int index, int count)
    {
        float pitch = _soundData[type].Pitch == 0 ? 1f : _soundData[type].Pitch;
        SoundStruct soundStruct = new SoundStruct(_soundData[type].Clip, _soundData[type].Volume, GetPitchForIndex(pitch, index, count));
        return Spawn(soundStruct);
    }

    public SoundObject Spawn (SoundStruct sound, int index, int count )
    {
        float pitch = sound.Pitch == 0 ? 1f : sound.Pitch;
        SoundStruct soundStruct = new SoundStruct(sound.Clip, sound.Volume, GetPitchForIndex(pitch, index, count));
        return Spawn(soundStruct);
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

    private float GetPitchForIndex (float basePitch, int index, int count, float range = 0.2f)
    {
        if (count <= 1)
            return basePitch;

        float t = (float) index / (count - 1);
        float multiplier = 1f + Mathf.Lerp(-range, range, t);
        return basePitch * multiplier;
    }
}