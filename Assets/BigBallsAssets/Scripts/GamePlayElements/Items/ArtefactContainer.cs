using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;

public class ArtefactContainer : IArtefactContainer
{
    private readonly IEffectFactory _effectFactory;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly Dictionary<IArtefactUser, UserType> _users;
    private List<ArtefactModel> _artefacts = new();

    public ArtefactContainer(
        IEffectFactory effectFactory,
        EnemySpawner enemySpawner,
        PlayerBallContainer playerBallContainer,
        StatHolder playerStatHolder,
        ArtefactsRepository artefactsRepository)
    {
        _effectFactory = effectFactory;
        _artefactsRepository = artefactsRepository;

        _users = new Dictionary<IArtefactUser, UserType>()
        {
            {enemySpawner, UserType.EnemySpawner},
            {playerBallContainer, UserType.Ball},
            {playerStatHolder, UserType.PlayerStats }
        };
    }

    public void Set(ItemModel item)
    {
        if (item is not HelmetModel buffer)
            return;

        foreach (var artefact in buffer.Artefacts)
        {
            ArtefactModel model = _artefactsRepository.AllModels[artefact.ArtefactType];
            Enable(model);
        }
    }

    public void Enable(ArtefactModel artefact)
    {
        if (artefact == null)
            return;

        _artefacts.Add(artefact);

        var effects = _effectFactory.Create(artefact);

        if (effects == null || effects.Count == 0)
            return;

        foreach (var user in _users)
        {
            if (user.Value == artefact.ArtefactConfig.UserType)
            {
                user.Key.AddEffects(effects);
            }
        }
    }
}