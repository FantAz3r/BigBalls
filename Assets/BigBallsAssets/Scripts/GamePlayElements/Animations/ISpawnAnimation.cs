using UnityEngine;

public interface ISpawnAnimation
{
    void PlaySpawn (Vector3 spawnPoint, bool wantResetTransform = true);
}