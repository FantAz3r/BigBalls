using System;
using BigBalls.Saves;

public interface ISaveService
{
    GameProgress GameProgress { get; }

    void Save (GameProgress progress);

    GameProgress Load ();
    void ResetSave ();
    void RegisterResetable (IResetble resetble);
}