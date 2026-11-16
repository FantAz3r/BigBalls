using System;

public interface ILiderboardService
{
    void SubmitScore (string leaderboardName, int score);

    event Action<string, int> OnScoreSubmitted;
}