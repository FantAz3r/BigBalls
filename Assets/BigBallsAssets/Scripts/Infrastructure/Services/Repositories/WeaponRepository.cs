using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.Services;
using System.Collections.Generic;

public class WeaponRepository : ItemRepository<ItemType, WeaponModel, WeaponConfig, WeaponSaveData>
{
    private readonly Dictionary<ItemType, WeaponConfig> _weaponsData = new();
    private readonly BallRepository _ballRepository;

    public WeaponRepository(IResourceLoader resourceLoader, ISaveService saveService, BallRepository ballRepository) : base(resourceLoader, saveService)
    {
        _weaponsData = resourceLoader.Load<CardsData>().Weapons;
        _ballRepository = ballRepository;
        Initialize();
    }

    protected override WeaponModel CreateModel(ItemType type, WeaponConfig config, CardSaveData saveData)
        => new WeaponModel(_ballRepository, (int)type, config, saveData.NoneGameLevel, saveData.ItemExp);

    protected override List<WeaponSaveData> GetSaveDataFromProgress() => GameProgress.Weapons;

    protected override Dictionary<ItemType, WeaponConfig> LoadConfigs() => _weaponsData;

    protected override void SaveGameProgress(List<WeaponSaveData> saveData) => GameProgress.Weapons = saveData;
}
