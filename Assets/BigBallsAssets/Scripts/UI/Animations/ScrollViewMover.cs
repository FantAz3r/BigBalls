using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

public class ScrollViewMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Mouse drag")]
    [SerializeField] private float _mouseDragSensitivity = 0.0025f;

    [Header("Screen edge scrolling")]
    [SerializeField] private float _edgeSize = 80f;
    [SerializeField] private float _edgeScrollSpeed = 0.5f;

    [Header("Touch")]
    [SerializeField] private float _touchSensitivity = 0.0025f;

    private bool _middleMouseDragging;
    private Vector2 _lastMousePosition;
    private IDeviceService _deviceService;

    private void Reset ()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    [Inject]
    public void Construct (IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    private void Update ()
    {
        if (_scrollRect == null)
            return;

        if (_deviceService.IsDecktop)
        {
            HandleMiddleMouseDrag();
            HandleScreenEdgeScrolling();
        }
        else
        {
            HandleTouch();
        }
    }

    private void HandleMiddleMouseDrag ()
    {
        if (Mouse.current == null)
            return;

        bool middleButtonPressed =
            Mouse.current.middleButton.IsPressed();

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        if (middleButtonPressed && !_middleMouseDragging)
        {
            _middleMouseDragging = true;
            _lastMousePosition = mousePosition;
        }

        if (!middleButtonPressed)
        {
            _middleMouseDragging = false;
            return;
        }

        Vector2 delta = mousePosition - _lastMousePosition;
        _lastMousePosition = mousePosition;

        MoveScroll(-delta * _mouseDragSensitivity);
    }

    private void HandleScreenEdgeScrolling ()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.middleButton.IsPressed())
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector2 screenSize = new Vector2(
            Screen.width,
            Screen.height
        );

        Vector2 direction = Vector2.zero;

        if (mousePosition.x <= _edgeSize)
            direction.x = -1f;
        else if (mousePosition.x >= screenSize.x - _edgeSize)
            direction.x = 1f;

        if (mousePosition.y <= _edgeSize)
            direction.y = -1f;
        else if (mousePosition.y >= screenSize.y - _edgeSize)
            direction.y = 1f;

        if (direction == Vector2.zero)
            return;

        MoveScroll(direction * _edgeScrollSpeed * Time.unscaledDeltaTime);
    }

    private void HandleTouch ()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.isPressed)
            return;

        Vector2 delta = touch.delta.ReadValue();

        MoveScroll(-delta * _touchSensitivity);
    }

    private void MoveScroll (Vector2 delta)
    {
        Vector2 normalizedDelta = new Vector2(
            _scrollRect.content.rect.width > _scrollRect.viewport.rect.width
                ? delta.x / _scrollRect.content.rect.width
                : 0f,

            _scrollRect.content.rect.height > _scrollRect.viewport.rect.height
                ? delta.y / _scrollRect.content.rect.height
                : 0f
        );

        Vector2 position = _scrollRect.normalizedPosition;

        position.x = Mathf.Clamp01(position.x + normalizedDelta.x);
        position.y = Mathf.Clamp01(position.y + normalizedDelta.y);

        _scrollRect.normalizedPosition = position;
    }
}
