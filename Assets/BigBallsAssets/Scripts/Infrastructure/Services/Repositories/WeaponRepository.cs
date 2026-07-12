using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Infrastructure.DI;
using BigBalls.Saves;
using BigBalls.Services;

public class WeaponRepository : ItemRepository<ItemType, WeaponModel, WeaponConfig, WeaponSaveData>
{
    private readonly Dictionary<ItemType, WeaponConfig> _weaponsData = new();
    private readonly BallRepository _ballRepository;

    public WeaponRepository (
        IResourceLoader resourceLoader,
        ISaveService saveService,
        BallRepository ballRepository,
        IObjectResolverProvider objectResolverProvider)
        : base(resourceLoader, saveService, objectResolverProvider)
    {
        _weaponsData = resourceLoader.Load<CardsData>().Weapons;
        _ballRepository = ballRepository;
        Initialize();
    }

    protected override WeaponModel CreateModel (ItemType type, WeaponConfig config, CardSaveData saveData)
        => new WeaponModel(_ballRepository, (int) type, config, saveData.NoneGameLevel, saveData.ItemExp);

    protected override List<WeaponSaveData> GetSaveDataFromProgress () => GameProgress.Weapons;

    protected override Dictionary<ItemType, WeaponConfig> LoadConfigs () => _weaponsData;

    protected override void SaveGameProgress (List<WeaponSaveData> saveData) => GameProgress.Weapons = saveData;
}
