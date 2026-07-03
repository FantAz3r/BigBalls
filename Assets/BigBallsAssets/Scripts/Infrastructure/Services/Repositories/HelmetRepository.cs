using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.Services;
using System.Collections.Generic;

public class HelmetRepository : ItemRepository<ItemType, HelmetModel, HelmetConfig, HelmetSaveData>
{
    private readonly ArtefactsRepository _artefactsRepository;
    private Dictionary<ItemType, HelmetConfig> _helmetData = new ();

    public HelmetRepository(IResourceLoader resourceLoader, ISaveService saveService, ArtefactsRepository artefactsRepository) : base(resourceLoader, saveService)
    {
        _helmetData = resourceLoader.Load<CardsData>().Helmets;
        _artefactsRepository = artefactsRepository;
    }

    protected override HelmetModel CreateModel(ItemType type, HelmetConfig config, CardSaveData saveData)
        => new HelmetModel(_artefactsRepository, (int) type, config, saveData.NoneGameLevel, saveData.ItemExp);

    protected override List<HelmetSaveData> GetSaveDataFromProgress() => GameProgress.Helmets;


    protected override Dictionary<ItemType, HelmetConfig> LoadConfigs() => _helmetData;

    protected override void SaveGameProgress(List<HelmetSaveData> saveData) => GameProgress.Helmets = saveData;
}
