using BigBalls.Saves;
using UnityEngine;
using YG;

public class YGSaveService : ISaveService
{
    public GameProgress GameProgress { get; private set; } = new();

    public void Save (GameProgress progress)
    {
        GameProgress = progress;
        YG2.saves.GameProgress = progress;
        YG2.SaveProgress();
    }
}
