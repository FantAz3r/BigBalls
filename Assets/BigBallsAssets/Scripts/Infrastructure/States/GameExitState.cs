public class GameExitState : IState
{
    private readonly BallRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly ISaveService _saveService;
    private readonly ILootFactory _lootFactory;

    public GameExitState(
        BallRepository ballsRepository,
        ArtefactsRepository artefactsRepository,
        ISaveService saveService,
        ILootFactory lootFactory)
    {
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
        _saveService = saveService;
        _lootFactory = lootFactory;
    }

    public void Enter()
    {
        if (_lootFactory != null)
        {
            _lootFactory.Dispose();
        }
        _saveService.Save();
    }

    public void Exit()
    {
    }
}