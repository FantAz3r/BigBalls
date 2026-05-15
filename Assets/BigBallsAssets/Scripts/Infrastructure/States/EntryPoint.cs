using System;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Infrastructure
{
    public class EntryPoint : IStartable
    {
        private IGameStateMachine _gameStateMachine;

        public EntryPoint(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine ?? throw new ArgumentNullException(nameof(gameStateMachine));

            Debug.Log(_gameStateMachine);
        }

        public void Start()
        {
            _gameStateMachine.EnterIn<LoadingLevelState, LevelID>(LevelID.MainMenu);
        }
    }
}
