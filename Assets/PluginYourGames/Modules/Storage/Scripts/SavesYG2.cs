using BigBalls.Saves;

namespace YG
{
    [System.Serializable]

    public partial class SavesYG
    {
        public int IdSave;
        public GameProgress GameProgress = new();
    }
}
