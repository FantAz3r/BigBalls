using UI_Spline_Renderer;
using UnityEngine;

public class BallTreeConnectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UISplineRenderer _splineRenderer;

    [Header("Visual Settings")]
    [SerializeField] private float _lineWidth = 2f;
    [SerializeField] private Color _lockedColor = Color.gray;
    [SerializeField] private Color _unlockedColor = Color.green;

    private RectTransform _from;
    private RectTransform _to;
    private Canvas _canvas;
    private RectTransform _canvasRect;
    private bool _isUnlocked;

    public bool IsUnlocked => _isUnlocked;

    private void Awake ()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("BallTreeConnectionUI: Canvas not found!");
            return;
        }
        _canvasRect = _canvas.transform as RectTransform;

        if (_splineRenderer == null)
        {
            Debug.LogError("BallTreeConnectionUI: UISplineRenderer not assigned!");
            return;
        }

        // Инициализация сплайна с двумя точками
        InitializeSpline();

        // Настройка ширины
        _splineRenderer.width = _lineWidth;
    }

    private void InitializeSpline ()
    {
       // // Получаем доступ к SplineContainer
       // var spline = _splineRenderer.splineContainer;
       // if (spline == null)
       // {
       //     Debug.LogError("SplineContainer is null!");
       //     return;
       // }
       //
       // _splineRenderer.splineContainer.Create();
       //
       // // Очищаем существующие точки
       // spline.Clear();
       //
       // // Добавляем две точки (используем float2 из Unity.Mathematics)
       // spline.Create; Add(new float2(0f, 0f));
       // spline.Add(new float2(100f, 100f)); // Временные координаты
       //
       // // Обновляем сплайн
       // spline.UpdateSpline();
    }

    public void SetConnection (RectTransform from, RectTransform to)
    {
        _from = from;
        _to = to;
        UpdatePositions();
    }

    public void UpdatePositions ()
    {
        if (_from == null || _to == null || _canvasRect == null || _splineRenderer == null)
            return;

        // Получаем локальные позиции в координатах Canvas
        Vector2 fromLocal = GetLocalPosition(_from);
        Vector2 toLocal = GetLocalPosition(_to);

        // Обновляем позиции точек в сплайне
        UpdateSplinePoints(fromLocal, toLocal);
    }

    private void UpdateSplinePoints (Vector2 from, Vector2 to)
    {
       // var spline = _splineRenderer.splineContainer;
       // if (spline == null || spline.Count < 2)
       // {
       //     // Если точек меньше 2, пересоздаём сплайн
       //     InitializeSpline();
       //     spline = _splineRenderer.splineContainer;
       // }
       //
       // // Устанавливаем позиции точек (конвертируем Vector2 в float2)
       // spline[0] = new float2(from.x, from.y);
       // spline[1] = new float2(to.x, to.y);
       //
       // // Обновляем сплайн для применения изменений
       // spline.UpdateSpline();
    }

    private Vector2 GetLocalPosition (RectTransform target)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            target.position,
            _canvas.worldCamera,
            out localPos
        );
        return localPos;
    }

    public void SetUnlocked (bool unlocked)
    {
        _isUnlocked = unlocked;

        if (_splineRenderer != null)
        {
            _splineRenderer.color = unlocked ? _unlockedColor : _lockedColor;
        }
    }
}
