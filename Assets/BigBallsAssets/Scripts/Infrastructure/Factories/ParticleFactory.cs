using BigBalls.Factories;
using BigBalls.Services;
using UnityEngine;

public class ParticleFactory : IParticleFactory
{
    private readonly IPoolService _poolService;

    public ParticleFactory(IPoolService poolService)
    {
        _poolService = poolService;
    }

    public ParticleObject Create(ParticleObject prefab, Vector3 position, float duration = 0)
    {
        var particle = _poolService.Get(prefab, position);
        SetupParticle(particle, duration);
        return particle;
    }

    public ParticleObject Create(ParticleObject prefab, Vector3 position, Quaternion rotation, float duration = 0)
    {
        var particle = _poolService.Get(prefab, position, rotation);
        SetupParticle(particle, duration);
        return particle;
    }

    public ParticleObject Create(ParticleObject prefab, Transform parent, float duration = 0)
    {
        var particle = _poolService.Get(prefab, parent);
        particle.transform.position = Vector3.zero;
        SetupParticle(particle, duration);
        return particle;
    }

    private void SetupParticle(ParticleObject particle, float duration = 0)
    {
        particle.Returned -= OnReturn;
        particle.Returned += OnReturn;
        particle.Play(duration);
    }

    public void OnReturn(ParticleObject particle)
    {
        if (particle == null)
            return;

        particle.Returned -= OnReturn;
        particle.ParticleSystem?.Clear(true);
        _poolService.Release(particle);
    }
}