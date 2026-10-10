using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.Factories;
using BigBalls.StaticData;
using UnityEngine;

public class MergeService : IMergeService
{
    private readonly BallRepository _ballRepository;
    private readonly IIdentifierService _identifierService;

    private PlayerCardHolder _cardHolder;

    public event Action<BallModel> MergeAvailable;
    public event Action<BallModel> Merged;

    public MergeService (
        BallRepository ballRepository,
        IIdentifierService identifierService)
    {
        _ballRepository = ballRepository;
        _identifierService = identifierService;
    }

    public void Init (PlayerCardHolder cardHolder)
    {
        _cardHolder = cardHolder;
    }

    public IReadOnlyList<BallModel> GetAvailableMerges ()
    {
        var result = new List<BallModel>();
        var ownedTypes = GetOwnedBallTypes();

        foreach (var kvp in _ballRepository.AllModels)
        {
            var ball = kvp.Value;

            if (ball.BallConfig.IsRoot)
            {
                continue;
            }

            if (ownedTypes.Contains(kvp.Key))
            {
                continue;
            }

            if (ball.BallConfig.ParentTypes == null || ball.BallConfig.ParentTypes.Count == 0)
            {
                continue;
            }

            if (ball.BallConfig.ParentTypes.All(p => ownedTypes.Contains(p)))
            {
                result.Add(ball);
            }
        }

        return result;
    }

    public bool CanMerge (BallModel target)
    {
        if (target?.BallConfig?.ParentTypes == null)
            return false;

        var ownedTypes = GetOwnedBallTypes();

        if (ownedTypes.Contains(target.BallConfig.BallType))
            return false;

        return target.BallConfig.ParentTypes.All(p => ownedTypes.Contains(p));
    }

    public bool TryMerge (BallModel target)
    {
        if (CanMerge(target) == false)
            return false;

        var parents = target.BallConfig.ParentTypes
            .Select(t => _ballRepository.AllModels[t])
            .ToList();

        foreach (var parent in parents)
            RemoveBall(parent);

        var newBall = new BallModel(_identifierService.ID, target.BallConfig);

        Merged?.Invoke(newBall);
        return true;
    }

    private HashSet<BallType> GetOwnedBallTypes ()
    {
        Debug.Log("Balls:");
        foreach(var ball in _cardHolder.Balls)
        {
            Debug.Log(ball.Config.name);
        }

        return _cardHolder.Balls
            .Select(b => b.BallConfig.BallType)
            .ToHashSet();
    }

    private void RemoveBall (BallModel ball)
    {
        _cardHolder.Remove(ball);
    }
}