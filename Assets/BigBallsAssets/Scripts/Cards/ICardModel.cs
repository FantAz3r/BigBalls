using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Saves;
using System;
using System.Collections.Generic;

public interface ICardModel
{
    int MaxLevel { get; }
    CardType Type { get; }
    ItemConfig Config { get; }
    int Level { get; }
    float ItemEXP { get; }
    bool IsOpen { get; }
    bool HasPlayer { get; }
    float EXPForNextLevel { get; }
    string Name { get; }
    string Description { get; }

    event Action<ICardModel> Upgraded;
    event Action<ICardModel> Changed;

    void AddItemEXP(float value);
    void AddToPlayer();
    CardSaveData CreateSaveData();
    List<List<string>> GetStatsText(bool isViewNextLevel);
    void InitFromData(CardSaveData data);
    void OpenItem();
    void Upgrade();
    void UpgradeNoneGameLevel();
}