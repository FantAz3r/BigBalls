using UnityEngine;
using BigBalls.GameplayObjects;

public class EnemySpawnerDebugView : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float offsetZ = 5f;
    [SerializeField] private Color fillColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] private Color wireColor = Color.red;

    // —сылку на спавнер нужно как-то передать (через DI-контейнер или вручную)
    private EnemySpawner _spawner;

    public void Init (EnemySpawner spawner) => _spawner = spawner;

    private void OnDrawGizmos ()
    {
        if (_spawner == null) return;

        // _fieldWidth и _rowIndex приватные Ч либо сделайте их internal/public,
        // либо добавьте свойства-геттеры в EnemySpawner
        float offsetX = Mathf.CeilToInt((_spawner.FieldWidth - 1) / 2f);

        foreach (var cell in _spawner.OccupiedPositions)
        {
            Vector3 center = new Vector3(
                cell.x - offsetX + cellSize * 0.5f,
                0.5f,
                offsetZ + cell.y
            );

            Gizmos.color = fillColor;
            Gizmos.DrawCube(center, new Vector3(cellSize, 0.1f, cellSize));

            Gizmos.color = wireColor;
            Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.1f, cellSize));
        }
    }
}
