using BigBalls.Configs;
using BigBalls.StaticData;

public interface ICard
{
    ItemConfig Config { get; }
    int Level { get; }
    float ItemEXP { get; }
    bool IsOpen { get; }

    CardSaveData CreateItemSave ();
    void InitFromData (CardSaveData data);
    void OpenItem ();
    void Upgrade ();
}