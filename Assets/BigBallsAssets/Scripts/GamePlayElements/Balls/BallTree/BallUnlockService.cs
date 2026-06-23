using BigBalls.StaticData;

public class BallUnlockService : IBallUnlockService
{
    private readonly BallsRepository _ballsRepository;

    public BallUnlockService (BallsRepository ballsRepository)
    {
        _ballsRepository = ballsRepository;
    }

    public bool UnlockBall (BallType type)
    {
        if (_ballsRepository.AllModels.ContainsKey(type))
            return false;

        var ballModel = _ballsRepository.GetModel(type);
        ballModel.OpenItem();

        return true;
    }
}
