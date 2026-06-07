using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public abstract class EffectBehaviour
    {
        protected EffectConfig Config;
        public Ball Host { get; protected set; }

        protected EffectBehaviour(EffectConfig config)
        {
            Config = config;
        }

        public void Subscribe(Ball host)
        {
            host.Hited += OnHit;
        }

        protected virtual void OnHit(int id)
        {
            Debug.Log("onHit");
        }

        public void Unsubscribe(Ball host) 
        {
            host.Hited -= OnHit;
        }
    }
}