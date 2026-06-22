using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;

public interface ICard
{
    ItemConfig Config { get; }
    int Level { get; }
    float ItemEXP { get; }
    bool IsOpen { get; }

    CardSaveData CraeateSaveData ();
    void InitFromData (CardSaveData data);
    void OpenItem ();
    void Upgrade ();
}