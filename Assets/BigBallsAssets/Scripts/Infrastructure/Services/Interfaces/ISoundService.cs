using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundService
{
    void Add (SoundObject soundObject);
    void Remove (SoundObject soundObject);
    void ContinueAll ();
    void PauseAll ();
    void StopAll ();
}
