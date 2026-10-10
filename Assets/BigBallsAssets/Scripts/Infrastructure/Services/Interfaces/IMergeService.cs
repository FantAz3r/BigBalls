using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;

public interface IMergeService
{
    event Action<BallModel> MergeAvailable;
    event Action<BallModel> Merged;

    IReadOnlyList<BallModel> GetAvailableMerges ();
    bool CanMerge (BallModel target);
    bool TryMerge (BallModel target);
    void Init (PlayerCardHolder cardHolder);
}
