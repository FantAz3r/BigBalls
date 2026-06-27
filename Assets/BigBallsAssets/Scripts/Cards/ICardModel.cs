using BigBalls.Configs;
using BigBalls.Saves;
using System;

public interface ICardModel
{
    ItemConfig Config { get; }
    int Level { get; }
    float ItemEXP { get; }
    bool IsOpen { get; }
    bool HasPlayer { get; }

    event Action<ICardModel> Upgraded;
    event Action<ICardModel> Changed;

    void AddItemEXP (float value);
    void AddToPlayer();
    CardSaveData CreateSaveData ();
    void InitFromData (CardSaveData data);
    void OpenItem ();
    void Upgrade ();
}