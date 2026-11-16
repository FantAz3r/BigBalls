using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    private const int MaxWidth = 15;

    [SerializeField] private List<FieldCell> _cells = new();
    [SerializeField] private List<GameObject> _lines = new();
    [field: SerializeField] public int Width { get; private set; } = 7;
    [field: SerializeField] public int Length { get; private set; } = 15;
    [field: SerializeField] public FieldCell CellPrefab { get; private set; }
    [field: SerializeField] public float CellSize { get; private set; } = 1f;

    public IReadOnlyList<FieldCell> Cells => _cells.Where(cell => cell.ActiveInHierarchy()).ToList();

    public IReadOnlyList<GameObject> Lines => _lines;

    public List<FieldCell> FreeCells => _cells.Where(cell => cell.ActiveInHierarchy() && cell.HasEnemy == false).ToList();

    public void SetWidth (int value)
    {
        if (value < 0)
            return;

        Width = Mathf.Min(value, MaxWidth);
        RefreshActiveLines();
    }

    private void RefreshActiveLines ()
    {
        int halfWidth = Width / 2;
        int minActiveX = -halfWidth;
        int maxActiveX = Width - halfWidth - 1;

        int halfMaxWidth = MaxWidth / 2;

        for (int i = 0; i < _lines.Count; i++)
        {
            int x = -halfMaxWidth + i;
            bool active = x >= minActiveX && x <= maxActiveX;
            _lines[i].SetActive(active);
        }
    }
}