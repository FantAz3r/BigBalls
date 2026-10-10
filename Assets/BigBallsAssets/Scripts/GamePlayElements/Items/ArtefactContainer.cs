using System.Collections.Generic;
using System.Linq;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

public class ArtefactContainer : IArtefactContainer
{
    private readonly IEffectFactory _effectFactory;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly Dictionary<IArtefactUser, UserType> _users;
    private readonly List<ArtefactModel> _artefacts = new();

    public ArtefactContainer (
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

    public IReadOnlyList<ArtefactModel> Artefacts => _artefacts;


    public void Enable (ArtefactModel artefact)
    {
        if (artefact == null) return;
        if (_artefacts.Contains(artefact)) return; 

        _artefacts.Add(artefact);
        ApplyToUsers(artefact, add: true);
    }

    public void Disable (ArtefactModel artefact)
    {
        if (artefact == null) return;
        if (!_artefacts.Remove(artefact)) return;

        ApplyToUsers(artefact, add: false);
    }

    public bool Has (ArtefactModel artefact) =>
        artefact != null && _artefacts.Contains(artefact);

    public void Clear ()
    {
        foreach (var artefact in _artefacts.ToList())
            Disable(artefact);
    }

    // ---------- INTERNAL ----------

    private void ApplyToUsers (ArtefactModel artefact, bool add)
    {
        foreach (var user in _users)
        {
            if (user.Value != artefact.ArtefactConfig.UserType)
                continue;

            if (add)
                user.Key.AddEffects(artefact);
            else
                user.Key.RemoveEffects(artefact);
        }
    }
}