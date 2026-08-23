using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

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

        Debug.Log("helmetSeted");

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

        foreach (var user in _users)
        {
            if (user.Value == artefact.ArtefactConfig.UserType)
            {
                user.Key.AddEffects(artefact);
            }
        }
    }
}