using BigBalls.Services;
using System;
using UnityEngine;
using YG;

namespace BigBalls.Infrastructure
{
    public class LoadingLevelState : IPayloadedState<LevelID>
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IUpdateService _updateService;
        private LevelID _currentLevel;
        private LevelID _previousLevel;

        public LoadingLevelState(
            IUpdateService updateService,
            ISceneLoader sceneLoader)
        {
            _updateService = updateService;
            _sceneLoader = sceneLoader;
        }

        public void Enter(LevelID level)
        {
            InitCurrentLevel(level);
        }

        public void Exit()
        {
            _updateService.Clear();
        }


        private void InitCurrentLevel(LevelID level)
        {
            _previousLevel = _currentLevel;
            _currentLevel = level;

            switch (level)
            {
                case LevelID.MainMenu:
                    _sceneLoader.LoadSceneWithLoadingScreen(level.ToString(), InitMainMenu);
                    YG2.GameReadyAPI();
                    break;

                case LevelID.Level1:
                    _sceneLoader.LoadSceneWithLoadingScreen(level.ToString(), InitGameLevel);
                    break;

                case LevelID.Level2:
                    _sceneLoader.LoadSceneWithLoadingScreen(level.ToString(), InitGameLevel);
                    break;

                case LevelID.Level3:
                    _sceneLoader.LoadSceneWithLoadingScreen(level.ToString(), InitGameLevel);
                    break;

                case LevelID.Level4:
                    _sceneLoader.LoadSceneWithLoadingScreen(level.ToString(), InitGameLevel);
                    break;

                case LevelID.Level5:
                    _sceneLoader.LoadSceneWithLoadingScreen(level.ToString(), InitGameLevel);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }

        private void InitMainMenu()
        {
            Debug.Log("main Menu");
        }


        private void InitGameLevel()
        {
            Debug.Log("InitGameLevel");
        }

        private void CommonInit()
        {
        }
    }
}