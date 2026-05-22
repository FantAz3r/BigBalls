using BigBalls.Infrastructure.DI;
using UnityEngine;

namespace BigBalls.Infrastructure
{
    public class LoadingLevelState : IPayloadedState<LevelID>
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IGameStateMachine _gameStateMachine;

        private LevelID _currentLevel;
        private LevelID _previousLevel;

        public LoadingLevelState(ISceneLoader sceneLoader, IGameStateMachine gameStateMachine)
        {
            
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
        }

        public void Enter(LevelID level)
        {
            Debug.Log("123");
            InitCurrentLevel(level);
        }

        public void Exit()
        {
        }

        private void InitCurrentLevel(LevelID level)
        {
            _previousLevel = _currentLevel;
            _currentLevel = level;

            if(level == LevelID.MainMenu)
            {
                _sceneLoader.LoadSceneImmediately(level.ToString(), InitMainMenu);
                return;
            }

            _sceneLoader.LoadSceneWithLoadingScreen(level.ToString(), InitGameLevel);
        }

        private void InitGameLevel()
        {
            _gameStateMachine.EnterIn<CreateLevelState>();
        }

        private void InitMainMenu()
        {
            Debug.Log("124");
            _gameStateMachine.EnterIn<MainMenuState>();
        }
    }
}