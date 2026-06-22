using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Services;
using BigBalls.Saves;

public class ArtefactsRepository : ItemRepository<ArtefactType, ArtefactModel, ArtefactConfig, ArtefactSaveData>, IArtefactsRepository
{
    private readonly Dictionary<ArtefactType, ArtefactConfig> _artefactsData;

    public ArtefactsRepository (IResourceLoader resourceLoader, ISaveService saveService)
        : base(resourceLoader, saveService)
    {
        _artefactsData = resourceLoader.Load<CardsData>().Artefacts;
        Initialize();
    }

    protected override Dictionary<ArtefactType, ArtefactConfig> LoadConfigs ()
        => _artefactsData;

    protected override ArtefactModel CreateModel (ArtefactType type, ArtefactConfig config, CardSaveData saveData)
        => new ArtefactModel((int) type, config, saveData.Level);

    protected override List<ArtefactSaveData> GetSaveDataFromProgress ()
        => GameProgress.Artefacts;

    protected override void UpdateGameProgress (List<ArtefactSaveData> saveData)
        => GameProgress.Artefacts = saveData;
}