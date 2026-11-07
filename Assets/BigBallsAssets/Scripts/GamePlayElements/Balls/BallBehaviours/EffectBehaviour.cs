using BigBalls.Configs;
using System;

namespace BigBalls.GameplayObjects
{
    public abstract class EffectBehaviour
    {
        protected int Level;
        protected EffectConfig Config;
        private IDisposable _disposable;

        public IEntity Host { get; protected set; }
        public Ball Ball => Host as Ball;

        protected EffectBehaviour(EffectConfig config, int level)
        {
            Config = config;
            Level = level;
        }

        protected abstract IDisposable SubscribeInternal(IEntity host);

        protected virtual void OnUnsubscribe(IEntity host = null) { }

        public void Subscribe(IEntity host = null)
        {
            Host = host;
            _disposable = SubscribeInternal(host);
        }

        public virtual void OnUpgrade(int level)
        {
            Level = level;
        }

        public void Unsubscribe(IEntity host = null)
        {
            if (_disposable != null)
            {
                _disposable.Dispose();
                _disposable = null;
            }

            OnUnsubscribe(host);
            Host = null;
        }
    }
}