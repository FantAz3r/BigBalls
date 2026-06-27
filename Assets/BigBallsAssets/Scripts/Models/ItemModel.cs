using System;
using BigBalls.Configs;
using BigBalls.Saves;

namespace BigBalls.StaticData
{
    public class ItemModel : ICardModel
    {
        private const int ItemMaxLevel = 10;
        private int _id;

        public ItemModel (int id, ItemConfig config, int level = 0)
        {
            _id = id;
            Config = config;
            NoneGameLevel = level;

            IsOpen = config.IsOpen;
        }

        public event Action<ICardModel> Upgraded;
        public event Action<ICardModel> Changed;
        public ItemConfig Config { get; private set; }
        public float ItemEXP { get; private set; }
        public bool IsOpen { get; private set; }
        public int Level => NoneGameLevel + InGameLevel;
        public int NoneGameLevel { get; private set; } = 0;
        public int InGameLevel { get; private set; }
        public bool HasPlayer { get; private set; } = false;

        public virtual CardSaveData CreateSaveData () => new CardSaveData(_id, IsOpen, ItemEXP, NoneGameLevel);
        public void Upgrade ()
        {
            if (Level < ItemMaxLevel)
            {
                InGameLevel++;
                Upgraded?.Invoke(this);
            }
        }

        public void AddToPlayer()
        {
            HasPlayer = true;
            Changed?.Invoke(this);
        }

        public void OpenItem()
        {
            IsOpen = true;
        }

        public void InitFromData (CardSaveData data)
        {
            _id = data.Id;
            NoneGameLevel = data.NoneGameLevel;
            IsOpen = data.IsOpen;
            ItemEXP = data.ItemExp;
        }

        public void AddItemEXP(float value)
        {
            ItemEXP += value;
        }
    }
}