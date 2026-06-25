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

    public void Save (GameProgress progress)
    {
        GameProgress = progress;
        YG2.saves.GameProgress = progress;
        YG2.SaveProgress();
    }

    public GameProgress Load () => YG2.saves.GameProgress;

    public void ResetSave ()
    {
        GameProgress.Clear();
        Save(GameProgress);

        foreach (var resetble in _resetbles)
        {
            resetble.Reset();
        }
    }
}
