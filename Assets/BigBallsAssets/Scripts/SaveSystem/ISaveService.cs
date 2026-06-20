using BigBalls.Saves;

public interface ISaveService
{
    GameProgress GameProgress { get; }

    void Save (GameProgress progress);
}