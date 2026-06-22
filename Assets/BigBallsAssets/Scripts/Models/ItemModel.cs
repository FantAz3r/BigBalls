using System;
using BigBalls.Configs;
using BigBalls.Saves;

namespace BigBalls.StaticData
{
    public class ItemModel : ICardModel
    {
        private const int ItemMaxLevel = 10;
        private int _id;

        public ItemModel (int id, ItemConfig config, int level = 1)
        {
            _id = id;
            Config = config;
            Level = level;
        }

        public event Action<ICardModel> Upgraded;
        public ItemConfig Config { get; private set; }
        public float ItemEXP { get; private set; }
        public bool IsOpen { get; private set; } = false;
        public int Level { get; private set; }

        public CardSaveData CraeateSaveData () => new CardSaveData(_id, IsOpen, ItemEXP, Level);
        public void Upgrade ()
        {
            if (Level < ItemMaxLevel)
            {
                Level++;
                Upgraded?.Invoke(this);
            }
        }

        public void OpenItem () => IsOpen = true;

        public void InitFromData (CardSaveData data)
        {
            _id = data.Id;
            Level = data.Level;
            IsOpen = data.IsOpen;
            ItemEXP = data.ItemExp;
        }
    }
}