using UnityEngine;

namespace BigBalls.Factories
{
    public interface IParticleFactory
    {
        ParticleSystem Create();
    }
}