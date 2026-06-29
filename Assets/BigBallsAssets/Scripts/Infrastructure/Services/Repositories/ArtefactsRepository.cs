using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.Services;
using System.Collections.Generic;

public class ArtefactsRepository : ItemRepository<ArtefactType, ArtefactModel, ArtefactConfig, ArtefactSaveData>, IArtefactsRepository
{
    private readonly Dictionary<ArtefactType, ArtefactConfig> _artefactsData;

    public ArtefactsRepository(IResourceLoader resourceLoader, ISaveService saveService)
        : base(resourceLoader, saveService)
    {
        _artefactsData = resourceLoader.Load<CardsData>().Artefacts;
        Initialize();
    }

    protected override Dictionary<ArtefactType, ArtefactConfig> LoadConfigs() => _artefactsData;

    protected override ArtefactModel CreateModel(ArtefactType type, ArtefactConfig config, CardSaveData saveData)
        => new ArtefactModel((int)type, config, saveData.NoneGameLevel, saveData.ItemExp);

    protected override List<ArtefactSaveData> GetSaveDataFromProgress() => GameProgress.Artefacts;

    protected override void SaveGameProgress(List<ArtefactSaveData> saveData) => GameProgress.Artefacts = saveData;
}