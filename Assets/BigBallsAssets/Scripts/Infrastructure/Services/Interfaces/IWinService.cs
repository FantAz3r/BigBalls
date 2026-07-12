using BigBalls.Infrastructure;

public interface IWinService
{
    void OnWin();

    void SetLevel(LevelID level);
    void SetWinReason(IWinReason winReason);
}