using BigBalls.Configs;

public interface IWinService
{
    void OnWin();
    void SetLevelConfig(LevelConfig config);
    void SetWinReason(IWinReason winReason);
}