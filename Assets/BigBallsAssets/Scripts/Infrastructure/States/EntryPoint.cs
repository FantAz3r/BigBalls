using UnityEngine;

public class EntryPoint : MonoBehaviour, ICoroutineRunner
{
    private GameStateMachine _gameStateMachine;

    private void Start()
    {
        _gameStateMachine = new GameStateMachine(new SceneLoader(this));
        _gameStateMachine.EnterIn<BootstrapState>();
        DontDestroyOnLoad(this); 
    }
}
