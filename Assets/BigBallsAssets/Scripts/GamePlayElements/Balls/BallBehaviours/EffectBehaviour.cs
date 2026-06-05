using BigBalls.StaticData;

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

        public void Init(Ball host)
        {
            Host = host;
            Host.Hited += OnHit;
        }

        protected virtual void OnHit(int id) { }

        protected virtual void OnDisable() 
        {
            Host.Hited -= OnHit;
        }
    }
}