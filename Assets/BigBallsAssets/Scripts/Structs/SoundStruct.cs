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
}
