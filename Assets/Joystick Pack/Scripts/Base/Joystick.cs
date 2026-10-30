using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float Horizontal { get { return snapX ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return snapY ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOptions { get { return axisOptions; } set { axisOptions = value; } }
    public bool SnapX { get { return snapX; } set { snapX = value; } }
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    [Header("Click Settings")]
    [Tooltip("Порог в пикселях, после которого нажатие считается перетаскиванием, а не кликом.")]
    [SerializeField] private float dragThreshold = 15f;

    [Header("Joystick Settings")]
    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0.1f;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX;
    [SerializeField] private bool snapY;

    [SerializeField] protected RectTransform background;
    [SerializeField] private RectTransform handle;
    private RectTransform baseRect;

    private Canvas canvas;
    private Camera cam;

    private Vector2 input = Vector2.zero;

    // Флаги состояния
    private bool _isPressed;
    private bool _isDragging;
    private Vector2 _pointerDownPosition;

    public event Action<Vector2> MovePerformed;
    public event Action MoveCanceled;
    public event Action Clicked;

    protected virtual void Start ()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown (PointerEventData eventData)
    {
        _isPressed = true;
        _isDragging = false;
        _pointerDownPosition = eventData.position;

        // Если нажали мимо ручки (на фон), сразу разрешаем драг
        if (!RectTransformUtility.RectangleContainsScreenPoint(handle, eventData.position, eventData.pressEventCamera))
        {
            _isDragging = true;
            UpdateJoystick(eventData);
        }
    }

    public void OnDrag (PointerEventData eventData)
    {
        if (!_isPressed) return;

        // Если мы еще не в режиме драга, проверяем, сдвинулся ли палец достаточно
        if (!_isDragging)
        {
            if (Vector2.Distance(_pointerDownPosition, eventData.position) > dragThreshold)
            {
                _isDragging = true; // Порог пройден, начинаем драг
            }
            else
            {
                return; // Палец двигается, но это пока считается "кликом", джойстик не трогаем
            }
        }

        // Обновляем позицию джойстика
        UpdateJoystick(eventData);
    }

    public virtual void OnPointerUp (PointerEventData eventData)
    {
        if (!_isPressed) return;

        _isPressed = false;

        // Если мы так и не начали драг - это был клик
        if (!_isDragging)
        {
            Clicked?.Invoke();
        }
        else
        {
            // Если был драг - сбрасываем ручку
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            MoveCanceled?.Invoke();
        }
    }

    private void UpdateJoystick (PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;

        Vector2 newInput = (eventData.position - position) / (radius * canvas.scaleFactor);

        input = newInput;
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;

        // Отправляем событие движения
        MovePerformed?.Invoke(Direction);
    }

    protected virtual void HandleInput (float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
            input = Vector2.zero;
    }

    private void FormatInput ()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat (float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                return (value > 0) ? 1 : -1;
            }

            if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                return (value > 0) ? 1 : -1;
            }
            return value;
        }

        if (value > 0)
            return 1;
        if (value < 0)
            return -1;
        return 0;
    }

    public void ForceReset ()
    {
        _isPressed = false;
        _isDragging = false;
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        MoveCanceled?.Invoke();
    }

    protected Vector2 ScreenPointToAnchoredPosition (Vector2 screenPosition)
    {
        Vector2 localPoint = new Vector2(0.5f, 0.5f);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }