using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Saves;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using YG;

namespace BigBalls.StaticData
{
    public abstract class ItemModel : ICardModel
    {
        private const string RuLanguage = "ru";
        private const string EnLanguage = "en";
        private const string TRLanguage = "tr";

        private int _id;
        private int _euqipmentLevel;
        public ItemModel(int id, ItemConfig config, int level = 0, float exp = 0)
        {
            _id = id;
            Config = config;
            NoneGameLevel = level;
            ItemEXP = exp;

            IsOpen = config.IsOpen;
        }

        public event Action<ICardModel> Upgraded;
        public event Action<ICardModel> Changed;

        public StatViewService StatViewService { get; set; }
        public ItemConfig Config { get; private set; }
        public float ItemEXP { get; private set; }
        public bool IsOpen { get; private set; }
        public int MaxLevel { get; private set; } = 10;
        public int MaxNoneGameLevel { get; private set; } = 5;
        public int NoneGameLevel { get; private set; } = 0;
        public int InGameLevel { get; private set; }
        public bool HasPlayer { get; private set; } = false;
        public float EXPForNextLevel => Config.BaseEXPForUpgrade * Mathf.Pow(Config.LevelEXPMultipy, NoneGameLevel);

        public CardType Type => Config.Type;
        public int Level => NoneGameLevel + InGameLevel + _euqipmentLevel;

        public string Name => OnCorrectLanguage(Config.NameRU, Config.NameEN, Config.NameTR);
        public string Description => OnCorrectLanguage(Config.DescriptionRU, Config.DescriptionEN, Config.DescriptionTR);

        [Inject]
        public void Construct()
        {
            //TranslateService
        }

        public void Upgrade()
        {
            if (Level < MaxLevel)
            {
                InGameLevel++;
                Upgraded?.Invoke(this);
                BindStats();
            }
        }

        public void UpgradeNoneGameLevel()
        {
            if (NoneGameLevel < MaxNoneGameLevel)
            {
                if (true)//ItemEXP >= EXPForNextLevel)
                {
                    ItemEXP -= EXPForNextLevel;
                    NoneGameLevel++;
                    Upgraded?.Invoke(this);
                    BindStats();
                }
            }
        }

        public void AddToPlayer()
        {
            HasPlayer = true;
            Changed?.Invoke(this);
        }

        public void OpenItem() => IsOpen = true;

        public void InitFromData(CardSaveData data)
        {
            _id = data.Id;
            NoneGameLevel = data.NoneGameLevel;
            IsOpen = data.IsOpen;
            ItemEXP = data.ItemExp;
        }

        public void AddItemEXP (float value)
        {
            ItemEXP += value;
        }

        public void AddEquipmentLevel(int level) => _euqipmentLevel = level;

        public virtual CardSaveData CreateSaveData() => new CardSaveData(_id, IsOpen, ItemEXP, NoneGameLevel);

        public virtual void BindStats() { }

        protected virtual List<ItemStat> GetStats() => null;

        public List<List<string>> GetStatsText(bool isViewNextLevel)
        {
            var stats = GetStats();
            var table = new List<List<string>>();

            if (stats == null || stats.Count == 0)
                return table;

            foreach (var stat in stats)
            {
                if (isViewNextLevel)
                {
                    string nextValue = (stat.NextValue - stat.Value).ToString();
                    table.Add(new List<string> { stat.Name, stat.Value.ToString(), nextValue });
                }
                else
                {
                    table.Add(new List<string> { stat.Name, stat.Value.ToString(), string.Empty });
                }
            }

            return table;
        }

        private string OnCorrectLanguage(string ru, string en, string tr)
        {
            string lang = YG2.lang;

            switch (lang)
            {
                case RuLanguage:
                    return ru;
                case EnLanguage:
                    return en;
                case TRLanguage:
                    return tr;
                default:
                    return string.Empty;
            }
        }
    }
}