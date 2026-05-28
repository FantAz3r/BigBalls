namespace BigBalls.GameplayObjects
{
    public abstract class Behaviour
    {
        private BehaviourType _type;
        public Ball Host { get; protected set; }

        protected Behaviour(BehaviourType type)
        {
            _type = type;
        }

        public void Init(Ball host)
        {
            Host = host;

            OnInit();
        }

        protected virtual void OnInit() { }
    }
}