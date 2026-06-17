using System;
using BigBalls.Configs;

namespace BigBalls.GameplayObjects
{
    public abstract class EffectBehaviour
    {
        protected readonly int Level;
        protected EffectConfig Config;
        private IDisposable _disposable;
        public IEntity Host { get; protected set; }

        protected EffectBehaviour (EffectConfig config, int level)
        {
            Config = config;
            Level = level;
        }

        protected abstract IDisposable SubscribeInternal (IEntity host);

        public void Subscribe (IEntity host)
        {
            if (_disposable != null)
                throw new ArgumentNullException(nameof(host));

            Host = host;
            _disposable = SubscribeInternal(host);
        }

        public void Unsubscribe (IEntity host)
        {
            _disposable.Dispose();
            _disposable = null;
            Host = null;
        }
    }
}