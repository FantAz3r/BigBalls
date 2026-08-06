using BigBalls.Factories;
using BigBalls.Services;
using UnityEngine;

public class ParticleFactory : IParticleFactory
{
   //private readonly IPoolService _poolService;
   //
   //public ParticleFactory (IPoolService poolService)
   //{
   //    _poolService = poolService;
   //}
   //
   //public ParticleObject Create (ParticleSystem prefab, Vector3 position)
   //{
   //    var particle = _poolService.GetObject<ParticleObject>(prefab.name, position);
   //    SetupParticle(particle, prefab);
   //    return particle;
   //}
   //
   //public ParticleObject Create (ParticleSystem prefab, Transform parent)
   //{
   //    var particle = _poolService.GetObject<ParticleObject>(prefab.name, parent);
   //    particle.transform.position = Vector3.zero;
   //    SetupParticle(particle, prefab);
   //    return particle;
   //}
   //
   //private void SetupParticle (ParticleObject particle, ParticleSystem prefab)
   //{
   //    particle.Returned -= OnReturn;
   //    particle.Returned += OnReturn;
   //
   //    particle.Play();
   //}
   //
   //public void OnReturn (ParticleObject particle)
   //{
   //    if (particle == null)
   //        return;
   //
   //    particle.Returned -= OnReturn;
   //    particle.ParticleSystem?.Clear(true);
   //    _poolService.ReleaseObject(particle);
   //}
}