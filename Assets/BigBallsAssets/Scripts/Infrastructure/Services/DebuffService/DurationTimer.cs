using System;
using System.Collections;
using BigBalls.GameplayObjects;
using UnityEngine;

public sealed class DurationTimer
{
    public IEnumerator Run (DebuffHandle handle, float duration, Action<bool> onComplete)
    {
        handle.Remaining = duration;

        while (handle.Remaining > 0f && !handle.Cancelled && !handle.Finished)
        {
            if (IsEntityAlive(handle.Entity) == false)
                break;

            handle.Remaining -= Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke(handle.Cancelled);
    }

    private static bool IsEntityAlive (IEntity entity)
    {
        if (entity is not UnityEngine.Object unityObj)
            return entity != null;

        if (unityObj == null)
            return false;

        Transform t = entity.Transform;
        return t != null && t.gameObject.activeSelf;
    }
}