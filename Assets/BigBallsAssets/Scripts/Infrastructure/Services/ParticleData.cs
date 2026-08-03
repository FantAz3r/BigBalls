using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Services
{

    [CreateAssetMenu(menuName = "Datas/ParticleData")]
    public class ParticleData : ScriptableObject
    {
        public List<ParticleObject> Particles = new List<ParticleObject>();
    }
}