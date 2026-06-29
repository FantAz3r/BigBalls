public class GameExitState : IState
{
    private readonly BallRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly ISaveService _saveService;
    private readonly GlobalWallet _globalWallet;

    public GameExitState(
        BallRepository ballsRepository,
        ArtefactsRepository artefactsRepository,
        ISaveService saveService,
        GlobalWallet globalWallet)
    {
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
        _saveService = saveService;
        _globalWallet = globalWallet;
    }

    public void Enter()
    {
        _globalWallet.Dispose();
        _saveService.Save();
    }

    public void Exit()
    {
    }
}
