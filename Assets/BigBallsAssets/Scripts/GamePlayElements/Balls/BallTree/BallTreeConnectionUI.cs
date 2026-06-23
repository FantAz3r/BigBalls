using UnityEngine;

public class BallTreeConnectionUI : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _lineWidth = 2f;
    [SerializeField] private Color _lockedColor = Color.gray;
    [SerializeField] private Color _unlockedColor = Color.green;
    [SerializeField] private float _zPosition = 0f; // Z-позиция для LineRenderer

    private RectTransform _from;
    private RectTransform _to;
    private Canvas _canvas;
    private bool _isUnlocked;

    private void Awake ()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("BallTreeConnectionUI: Canvas not found in parents!");
        }
    }

    public void SetConnection (RectTransform from, RectTransform to)
    {
        _from = from;
        _to = to;
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;

        // Убеждаемся, что LineRenderer использует мировые координаты
        _lineRenderer.useWorldSpace = true;

        UpdatePositions();
    }

    public void UpdatePositions ()
    {
        if (_from == null || _to == null || _canvas == null)
            return;

        // Получаем мировые позиции RectTransform
        var fromWorldPos = GetWorldPosition(_from);
        var toWorldPos = GetWorldPosition(_to);

        // Устанавливаем позиции с учетом Z
        fromWorldPos.z = _zPosition;
        toWorldPos.z = _zPosition;

        _lineRenderer.SetPosition(0, fromWorldPos);
        _lineRenderer.SetPosition(1, toWorldPos);
    }

    private Vector3 GetWorldPosition (RectTransform rectTransform)
    {
        // Способ 1: Через RectTransformUtility
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _canvas.transform as RectTransform,
            rectTransform.position,
            _canvas.worldCamera,
            out worldPos
        );
        return worldPos;

        // Способ 2: Альтернативный - через Transform
        // return rectTransform.TransformPoint(rectTransform.rect.center);

        // Способ 3: Через RectTransformUtility с экранными координатами
        // Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, rectTransform.position);
        // return _canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
    }

    public void SetUnlocked (bool unlocked)
    {
        _isUnlocked = unlocked;
        _lineRenderer.startColor = unlocked ? _unlockedColor : _lockedColor;
        _lineRenderer.endColor = unlocked ? _unlockedColor : _lockedColor;
    }
}
