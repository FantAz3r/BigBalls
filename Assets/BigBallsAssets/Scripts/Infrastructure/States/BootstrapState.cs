using UnityEngine;

namespace BigBalls.Infrastructure
{
    public class BootstrapState : IState
    {
        private IGameStateMachine _stateMachine;

        public BootstrapState(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
        }

        public void Exit()
        {

        }

        private void RegisterServices()
        {

        }
    }
}
