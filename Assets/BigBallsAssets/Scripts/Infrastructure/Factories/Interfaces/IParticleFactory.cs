using UnityEngine;

namespace BigBalls.Factories
{
    public interface IParticleFactory
    {
        ParticleObject Create (ParticleObject prefab, Vector3 position, float duration = 0);
        ParticleObject Create(ParticleObject prefab, Vector3 position, Quaternion rotation, float duration = 0);
        ParticleObject Create(ParticleObject prefab, Transform parent, float duration = 0);
    }
}