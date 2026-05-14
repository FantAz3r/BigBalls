using System;
using UnityEngine;

public class LoadingLevelState : IPayloadedState<LevelID>
{
    private readonly ISceneLoader _sceneLoader;
    private LevelID _currentLevel;
    private LevelID _previousLevel;

    public LoadingLevelState(ISceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void Enter(LevelID level)
    {
        InitCurrentLevel(level);
    }

    public void Exit()
    {
    }
   

    private void InitCurrentLevel(LevelID level)
    {
        _previousLevel = _currentLevel;
        _currentLevel = level;

        switch (level)
        {
            case LevelID.MainMenu:
                _sceneLoader.Load(level.ToString(), InitMainMenu, false);
                //YG2.GameReadyAPI();
                break;

            case LevelID.Level1:
                _sceneLoader.Load(level.ToString(), InitGameLevel, true);
                break;

            case LevelID.Level2:
                _sceneLoader.Load(level.ToString(), InitGameLevel, true);
                break;

            case LevelID.Level3:
                _sceneLoader.Load(level.ToString(), InitGameLevel, true);
                break;

            case LevelID.Level4:
                _sceneLoader.Load(level.ToString(), InitGameLevel, true);
                break;

            case LevelID.Level5:
                _sceneLoader.Load(level.ToString(), InitGameLevel, true);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(level));
        }
    }

    private void InitMainMenu()
    {
        Debug.Log("InitMainMenu");
    }

    private void InitGameLevel()
    {

      
    }

    private void CommonInit()
    {
    }
}