using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundService : ISoundService
{
    private HashSet<SoundObject> _soundObjects = new();

    public void Add (SoundObject soundObject)
    {
        _soundObjects.Add(soundObject);
    }

    public void Remove (SoundObject soundObject)
    {
        _soundObjects.Remove(soundObject);
    }

    public void PauseAll ()
    {
        foreach (var item in _soundObjects.ToList())
        {
            item.PauseSound();
        }
    }

    public void StopAll ()
    {
        foreach (var item in _soundObjects.ToList())
        {
            item.StopSound();
        }
    }

    public void ContinueAll ()
    {
        foreach (var item in _soundObjects.ToList())
        {
            item.ContinueSound();
        }
    }
}
