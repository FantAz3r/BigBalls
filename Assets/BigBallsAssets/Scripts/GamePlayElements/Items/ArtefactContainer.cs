using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

public class ArtefactContainer : IArtefactContainer
{
    private readonly IEffectFactory _effectFactory;
    private readonly Dictionary<IArtefactUser, UserType> _users;
    private List<ArtefactModel> _artefacts = new();

    public ArtefactContainer (IEffectFactory effectFactory, EnemySpawner enemySpawner, PlayerBallContainer playerBallContainer, StatHolder playerStatHolder)
    {
        _effectFactory = effectFactory;

        _users = new Dictionary<IArtefactUser, UserType>()
        {
            {enemySpawner, UserType.EnemySpawner},
            { playerBallContainer, UserType.Ball},
            {playerStatHolder, UserType.PlayerStats }
        };
    }

    public void Set (ItemModel item)
    {
        if (item is not IBuffer buffer)
            return;

        foreach (var artefact in buffer.Artefacts)
        {
            Enable(artefact);
        }
    }

    public void Enable (ArtefactModel artefact)
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