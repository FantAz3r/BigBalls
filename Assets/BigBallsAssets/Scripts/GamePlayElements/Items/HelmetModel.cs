using BigBalls.Configs;
using BigBalls.StaticData;
using System.Collections.Generic;

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

    public List<ArtefactModel> GetArtefacts()
    {
        List<ArtefactModel> atrefacts = new();

        foreach (var config in HelmetConfig.Artefacts)
        {
            ArtefactModel artefact = _artefactsRepository.AllModels[config.ArtefactType];
            artefact.AddEquipmentLevel(Level);
            atrefacts.Add(artefact);
        }

        return atrefacts;
    }
}
