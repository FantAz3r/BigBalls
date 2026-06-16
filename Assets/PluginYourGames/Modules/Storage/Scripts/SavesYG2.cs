using System.Collections.Generic;

namespace YG
{
    [System.Serializable]

    public partial class SavesYG
    {
        public int IdSave;
        public int Gold;

        public List<ItemSaveData> Items;
        public List<BallSaveData> Balls;
    }
}
