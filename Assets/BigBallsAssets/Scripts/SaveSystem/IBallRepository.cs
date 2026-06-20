using System.Collections.Generic;
using BigBalls.StaticData;

public interface IBallRepository
{
    Dictionary<BallType, BallModel> BallModels { get; }

    void AddOrUpdateBallModel (BallModel ballModel);
    BallModel GetBallModel (BallType type);
    void LoadBallData ();
    void SaveBallData ();
}