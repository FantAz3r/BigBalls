using UnityEngine;

namespace BigBalls.Factories
{
    public interface IParticleFactory
    {
        ParticleObject Create (ParticleObject prefab, Vector3 position);
    }
}