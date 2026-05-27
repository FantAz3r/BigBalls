public abstract class Behaviour
{
    private BehaviourType EffectType;
    public Ball Host { get; protected set; }

    public void Init(Ball host)
    {
        Host = host;
        OnInit();
    }

    protected virtual void OnInit() { }
}
