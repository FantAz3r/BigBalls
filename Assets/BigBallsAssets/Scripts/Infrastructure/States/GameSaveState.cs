using UnityEngine;

public class GameSaveState : IState
{
    private readonly BallRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly ISaveService _saveService;

    public GameSaveState(
        BallRepository ballsRepository,
        ArtefactsRepository artefactsRepository,
        ISaveService saveService)
    {
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
        _saveService = saveService;
    }

    public void Enter()
    {
        _saveService.Save();
    }

    public void Exit()
    {

    }
}