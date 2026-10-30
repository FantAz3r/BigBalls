using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;

public class HelmetModel : ItemModel, IBuffer
{
    private readonly ArtefactsRepository _artefactsRepository;

    public HelmetModel(
        ArtefactsRepository artefactsRepository,
        int id,
        HelmetConfig config,
        int level = 0,
        float exp = 0)
        : base(id, config, level, exp)
    {
        _artefactsRepository = artefactsRepository;
        HelmetConfig = config;
    }

    public HelmetConfig HelmetConfig { get; private set; }
    public List<ArtefactConfig> Artefacts => HelmetConfig.Artefacts;

    public override CardSaveData CreateSaveData () => new HelmetSaveData((int) HelmetConfig.ItemType, IsOpen, ItemEXP, NoneGameLevel, HasPlayer);

    public List<ArtefactModel> GetArtefacts()
    {
        List<ArtefactModel> atrefacts = new();

        foreach (var config in HelmetConfig.Artefacts)
        {
            ArtefactModel artefact = _artefactsRepository.AllModels[config.ArtefactType];
            atrefacts.Add(artefact);
        }

        return atrefacts;
    }

    public void AddEquipmentLevel ()
    {
        foreach (var config in HelmetConfig.Artefacts)
        {
            ArtefactModel artefact = _artefactsRepository.AllModels[config.ArtefactType];
            artefact.SetEquipmentLevel(Level);
        }
    }

    public void RemoveEquipmentLevel ()
    {
        foreach (var config in HelmetConfig.Artefacts)
        {
            ArtefactModel artefact = _artefactsRepository.AllModels[config.ArtefactType];
            artefact.SetEquipmentLevel(0);
        }
    }
}
