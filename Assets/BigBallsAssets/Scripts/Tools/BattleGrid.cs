using UnityEngine;

public static class BattleGrid
{
    public static Vector3 CellCenter (Vector3 origin, int x, int y, float cell)
    {
        return origin + new Vector3((x + 0.5f) * cell, 0f, (y + 0.5f) * cell);
    }

    public static Vector3 GridCenter (Vector3 origin, int width, int height, float cell)
    {
        return origin + new Vector3(width * 0.5f * cell, 0f, height * 0.5f * cell);
    }
}