using BigBalls.Saves;

public interface ISaveService
{
    GameProgress GameProgress { get; }
    void Save();
    void Load();
    void ResetSave();
    void RegisterResetable(IResetble resetble);
}