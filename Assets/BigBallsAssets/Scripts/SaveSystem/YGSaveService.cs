using System.Collections.Generic;
using BigBalls.Saves;
using YG;

public class YGSaveService : ISaveService
{
    private readonly HashSet<IResetble> _resetbles = new();
    public GameProgress GameProgress { get; private set; } = new();

    public void RegisterResetable (IResetble resetble)
    {
        _resetbles.Add(resetble);
    }

    public void Save ()
    {
        YG2.saves.GameProgress = GameProgress;
        YG2.SaveProgress();
    }

    public void Load ()
    {
        if (YG2.saves.GameProgress != null)
            GameProgress = YG2.saves.GameProgress;
    }

    public void ResetSave ()
    {
        GameProgress.Clear();
        Save();

        foreach (var resetble in _resetbles)
        {
            resetble.Reset();
        }
    }
}
