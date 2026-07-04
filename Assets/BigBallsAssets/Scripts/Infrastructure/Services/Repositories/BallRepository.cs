using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Infrastructure.DI;
using BigBalls.Saves;
using BigBalls.Services;
using BigBalls.StaticData;

public class BallRepository : ItemRepository<BallType, BallModel, BallConfig, BallSaveData>
{
    private readonly Dictionary<BallType, BallConfig> _ballsData;

    public BallRepository (IResourceLoader resourceLoader, ISaveService saveService, IObjectResolverProvider objectResolverProvider)
        : base(resourceLoader, saveService, objectResolverProvider)
    {
        _ballsData = resourceLoader.Load<CardsData>().Balls;
        Initialize();
    }

    protected override Dictionary<BallType, BallConfig> LoadConfigs ()
        => _ballsData;

    protected override BallModel CreateModel (BallType type, BallConfig config, CardSaveData saveData)
        => new BallModel((int) type, config, saveData.NoneGameLevel, saveData.ItemExp);

    protected override List<BallSaveData> GetSaveDataFromProgress ()
        => GameProgress.Balls;

    protected override void SaveGameProgress (List<BallSaveData> saveData)
        => GameProgress.Balls = saveData;
}