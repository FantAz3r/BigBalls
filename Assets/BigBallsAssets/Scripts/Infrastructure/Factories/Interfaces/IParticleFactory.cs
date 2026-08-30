using UnityEngine;

namespace BigBalls.Factories
{
    public interface IParticleFactory
    {
        ParticleObject Create (ParticleObject prefab, Vector3 position);
        ParticleObject Create(ParticleObject prefab, Vector3 position, Quaternion rotation);
        ParticleObject Create(ParticleObject prefab, Transform parent);
    }
}