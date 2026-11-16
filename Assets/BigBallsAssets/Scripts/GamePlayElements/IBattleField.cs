using System.Collections.Generic;
using BigBalls.Factories;
using UnityEngine;

public interface IBattleField
{
    /// Текущая ширина дороги (столбцы).
    int Width { get; }

    /// Сколько клеток по Z сейчас «видно» (обычно 15).
    int VisibleDepth { get; }

    /// Текущий мировой Z, где находится ближний к игроку тайл (спавн новой волны).
    float FrontZ { get; }

    /// Текущий мировой Z, где находится дальний от игрока тайл (игрок его прошёл).
    float BackZ { get; }

    /// Мировой X центра столбца column (0..Width-1).
    float ColumnToWorldX (int column);

    /// Столбец по мировому X.
    int WorldXToColumn (float worldX);

    /// Мировая позиция центра клетки (column, depth), где depth — «глубина» от FrontZ в сторону BackZ.
    Vector3 CellToWorld (int column, int depth);

    /// Все видимые клетки (column, depth) для случайного выбора способностями.
    IReadOnlyList<Vector2Int> GetVisibleCells ();

    /// Мировой центр клетки, но безопасно (clamped) в текущие границы.
    Vector3 GetRandomVisibleCellWorld (System.Random rng);
}