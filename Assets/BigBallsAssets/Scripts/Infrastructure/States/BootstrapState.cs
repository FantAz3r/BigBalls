using UnityEngine;

namespace BigBalls.Infrastructure
{
    public class BootstrapState : IState
    {
        private IGameStateMachine _stateMachine;

        public BootstrapState(IGameStateMachine stateMachine)
        {
            Debug.Log("BootstrapState");
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            Debug.Log("sm,dgkodfmgh");
        }

        public void Exit()
        {

        }

        private void RegisterServices()
        {

        }
    }
}
