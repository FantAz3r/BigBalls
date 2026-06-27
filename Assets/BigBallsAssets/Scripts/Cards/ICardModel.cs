using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;

public interface ICardModel
{
    ItemConfig Config { get; }
    int Level { get; }
    float ItemEXP { get; }
    bool IsOpen { get; }
    bool HasPlayer { get; }

    void AddItemEXP (float value);
    CardSaveData CreateSaveData ();
    void InitFromData (CardSaveData data);
    void OpenItem ();
    void Upgrade ();
}