using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundStruct
{
    public AudioClip Clip;
    public float Volume;
    public float Pitch;
    public bool NeedRandomPitch;

    public SoundStruct (AudioClip clip, float volume, float startPitch, bool needRandomPitch = false)
    {
        Clip = clip;
        Volume = volume;
        Pitch = startPitch;
        NeedRandomPitch = needRandomPitch;
    }
}
