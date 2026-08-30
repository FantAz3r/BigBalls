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

    public ParticleObject Create(ParticleObject prefab, Vector3 position)
    {
        var particle = _poolService.GetObject(prefab, position);
        SetupParticle(particle);
        return particle;
    }

    public ParticleObject Create(ParticleObject prefab, Vector3 position, Quaternion rotation)
    {
        var particle = _poolService.GetObject(prefab, position, rotation);
        SetupParticle(particle);
        return particle;
    }

    public ParticleObject Create(ParticleObject prefab, Transform parent)
    {
        var particle = _poolService.GetObject(prefab, parent);
        particle.transform.position = Vector3.zero;
        SetupParticle(particle);
        return particle;
    }

    private void SetupParticle(ParticleObject particle)
    {
        particle.Returned -= OnReturn;
        particle.Returned += OnReturn;
        particle.Play();
    }

    public void OnReturn(ParticleObject particle)
    {
        if (particle == null)
            return;

        particle.Returned -= OnReturn;
        particle.ParticleSystem?.Clear(true);
        _poolService.ReleaseObject(particle);
    }
}