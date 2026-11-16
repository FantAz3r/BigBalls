using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;

public sealed class DotTicker
{
    private readonly IDamageService _damageService;
    private const float DamageTickInterval = 0.5f;

    public DotTicker (IDamageService damageService)
    {
        _damageService = damageService;
    }

    public IEnumerator Run (DebuffHandle handle)
    {
        while (handle.HasContributions && !handle.Cancelled)
        {
            if (handle.Entity == null || IsEntityAlive(handle.Entity) == false)
                break;

            TickAll(handle, Time.deltaTime);
            handle.RemoveFinishedContributions();
            yield return null;
        }

        handle.Cancel();
    }

    private void TickAll (DebuffHandle handle, float dt)
    {
        foreach (var c in handle.Contributions)
        {
            if (c.Remaining <= 0f || c.Finished) continue;

            c.Remaining -= dt;
            c.TickAccumulator += dt;

            while (c.TickAccumulator >= DamageTickInterval && !c.Finished)
            {
                c.TickAccumulator -= DamageTickInterval;
                _damageService.ApplyDamage(handle.Entity, c.Dps, c.Color);
                c.AccumulatedDamage += c.Dps;

                if (c.Remaining <= 0f)
                {
                    c.Finish(cancelled: false);
                    break;
                }
            }
        }
    }

    private static bool IsEntityAlive (IEntity entity)
        => entity != null && entity.Transform != null && entity.Transform.gameObject.activeSelf;
}