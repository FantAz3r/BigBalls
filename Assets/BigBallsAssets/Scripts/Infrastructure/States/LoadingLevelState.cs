using System;
using YG;

public class LoadingLevelState : IPayloadedState<LevelID>
{
    private readonly ICoroutineRunner _coroutineRunner;

    private SceneLoader _sceneLoader;
    //private IGameFactory _gameFactory;
    //private IWindowService _windowService;
    private LevelID _currentLevel;
    private LevelID _previousLevel;

    public LoadingLevelState(ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void Enter(LevelID level)
    {
        CreateFactories();
        InitCurrentLevel(level);

        
    }

    public void Exit()
    {
    }

    private void CreateFactories()
    {
        _sceneLoader = new SceneLoader(_coroutineRunner);
    }

    private void InitCurrentLevel(LevelID level)
    {
        _previousLevel = _currentLevel;
        _currentLevel = level;

        switch (level)
        {
            case LevelID.MainMenu:
                _sceneLoader.Load(level.ToString(), InitMainMenu, false);
                YG2.GameReadyAPI();
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
        CommonInit();
       // _windowService.Open(WindowType.Background);
       // _windowService.Open(WindowType.MainMenu);
    }

    private void InitGameLevel()
    {
        CommonInit();

       //_gameFactory.SetSceneContainer();
       //_gameFactory.SetLevelConfig(_currentLevel);
       //_gameFactory.CreatePlayer(_previousLevel);
       //_gameFactory.CreateScoreCounter();
       //_gameFactory.CreateCamera();
       //_gameFactory.CreateEventSystem();
       //_gameFactory.CreateCardSelector();
       //_gameFactory.CreateLight();
       //_gameFactory.CreateEnemies();
       //_windowService.CreateJoystick();
       //_gameFactory.CreatePortalsFactory();
       //_gameFactory.CreateQuests();
       //_gameFactory.CreateQuestRuner();
       //
       //_windowService.Open(WindowType.HUD);
       //_windowService.Open(WindowType.WaveViewer);
       //_gameFactory.RunLevel();
    }

    private void InitTowerLevel()
    {
        CommonInit();

       // _gameFactory.SetLevelConfig(_currentLevel);
       // _gameFactory.CreateLight();
       // _gameFactory.CreateTower();
       // _gameFactory.CreatePlayer(_previousLevel);
       // _gameFactory.CreateScoreCounter();
       // _gameFactory.CreateEventSystem();
       // _gameFactory.CreateCardSelector();
       //
       // _windowService.CreateJoystick();
       // _gameFactory.CreateCamera();
       // _gameFactory.CreatePortalsFactory();
       // _gameFactory.CreateQuests();
       // _gameFactory.CreateQuestRuner();
       //
       // _windowService.Open(WindowType.HUD);
       //
       // _gameFactory.RunLevel();
    }

    private void CommonInit()
    {
       // _gameFactory.SetCurrentLevel(_currentLevel);
       // _gameFactory.CreateSpawners();
       // _windowService.CreateUIRoot();
       // _gameFactory.CreateBackgroundSounds();
    }
}