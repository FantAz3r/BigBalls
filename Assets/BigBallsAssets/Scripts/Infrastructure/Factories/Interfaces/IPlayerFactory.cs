using BigBalls.GameplayObjects;

namespace BigBalls.Factories
{
    public interface IPlayerFactory
    {
        Player Create();
        void DeathSubscribe ();
        void OnPlayerSpawned ();
    }
}