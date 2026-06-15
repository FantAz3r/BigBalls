using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public abstract class EffectBehaviour
    {
        protected EffectConfig Config;
        protected readonly int Level;

        public Ball Host { get; protected set; }

        protected EffectBehaviour(EffectConfig config, int level)
        {
            Config = config;
            Level = level;
        }

        public void Subscribe(Ball host)
        {
            Host = host;
            host.Hited += OnHit;
        }

        protected virtual void OnHit(int id)
        {
            Debug.Log("onHit");
        }

        public void Unsubscribe(Ball host) 
        {
            host.Hited -= OnHit;
            Host = null;
        }
    }
}