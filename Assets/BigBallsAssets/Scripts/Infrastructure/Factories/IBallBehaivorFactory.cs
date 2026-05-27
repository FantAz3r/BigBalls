namespace BigBalls.Factories
{
    public interface IBallBehaivorFactory
    {
        Behaviour Get(BehaviourType type);
    }
}