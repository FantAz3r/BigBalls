using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private ButtonStateSettings _normal;
    [SerializeField] private ButtonStateSettings _highlighted;
    [SerializeField] private ButtonStateSettings _pressed;

    private Image _targetImage;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private RectTransformSnapshot _originRect;
    private CanvasGroupSnapshot _originCanvasGroup;

    private ButtonState _currentState;
    private Tween _normalTween;
    private Tween _highlightedTween;
    private Tween _pressedTween;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private IEnumerator Start()
    {
        yield return null;

        _originRect = new RectTransformSnapshot(_rectTransform);
        _originCanvasGroup = new CanvasGroupSnapshot(_canvasGroup);

        _normalTween = ConstructTween(_normal);
        _highlightedTween = ConstructTween(_highlighted);
        _pressedTween = ConstructTween(_pressed, () => _normalTween.Restart());

        SetState(ButtonState.Normal, instant: true);
    }

    private void OnDisable()
    {
        TrySetState(ButtonState.Normal, instant: true);
    }

    private void OnDestroy()
    {
        _normalTween.Kill();
        _highlightedTween.Kill();
        _pressedTween.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TrySetState(ButtonState.Highlighted);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TrySetState(ButtonState.Normal);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TrySetState(ButtonState.Pressed);
    }

    private void TrySetState(ButtonState newState, bool instant = false)
    {
        if (newState == _currentState)
        {
            return;
        }

        Tween currentAnimation = GetStateAnimation(_currentState);

        if ((currentAnimation.IsComplete() == false) && (_currentState == ButtonState.Pressed))
        {
            return;
        }

        SetState(newState, instant);
    }

    private void SetState(ButtonState newState, bool instant = false)
    {
        Tween currentAnimation = GetStateAnimation(_currentState);

        if (currentAnimation.IsComplete() == false)
        {
            currentAnimation.Complete();
        }

        _currentState = newState;
        currentAnimation = GetStateAnimation(_currentState);
        currentAnimation.Restart();

        if (instant)
        {
            currentAnimation.Complete(withCallbacks: true);
        }
    }

    private Tween ConstructTween(ButtonStateSettings settings, Action onComplete = null)
    {
        Sequence sequence = DOTween.Sequence();

        float duration = settings.AnimationSettings.Duration;
        Ease ease = settings.AnimationSettings.Ease;
        bool interactable = settings.AnimationSettings.InteractableWhenPlay;
        bool blocksRaycast = settings.AnimationSettings.BlockRaycastWhenPlay;
        bool ignoreParentGroup = settings.AnimationSettings.IgnoreParentGroupWhenPlay;

        if (settings.AnimationSettings.TryGetTargetScale(out Vector3 targetScale) == false)
        {
            targetScale = _originRect.Scale;
        }

        if (settings.AnimationSettings.TryGetAnchoredOffset(out Vector2 anchoredOffset) == false)
        {
            anchoredOffset = Vector2.zero;
        }

        if (settings.AnimationSettings.TryGetTargetRotation(out Vector3 targetRotation) == false)
        {
            targetRotation = _originRect.Rotation;
        }

        if (settings.AnimationSettings.TryGetTargetAlpha(out float targetAlpha) == false)
        {
            targetAlpha = _originCanvasGroup.Alpha;
        }

        sequence.Append(_rectTransform.DOScale(targetScale, duration).SetEase(ease))
            .Join(_rectTransform.DOAnchorPos(_originRect.AnchoredPosition + anchoredOffset, duration).SetEase(ease))
            .Join(_rectTransform.DORotate(targetRotation, duration).SetEase(ease))
            .Join(_canvasGroup.DOFade(targetAlpha, duration).SetEase(ease))
            .Join(_targetImage.DOColor(settings.Color, duration).SetEase(ease))
            .SetDelay(settings.AnimationSettings.Delay)
            .SetAutoKill(false)
            .OnPlay(() =>
            {
                _canvasGroup.interactable = interactable;
                _canvasGroup.blocksRaycasts = blocksRaycast;
                _canvasGroup.ignoreParentGroups = ignoreParentGroup;
            })
            .OnComplete(() =>
            {
                _targetImage.sprite = settings.Sprite;
                _canvasGroup.interactable = _originCanvasGroup.Interactable;
                _canvasGroup.blocksRaycasts = _originCanvasGroup.BlocksRaycasts;
                _canvasGroup.ignoreParentGroups = _originCanvasGroup.IgnoreParentGroups;
                onComplete?.Invoke();
            });

        sequence.Rewind();

        return sequence;
    }

    private Tween GetStateAnimation(ButtonState state)
    {
        return state switch
        {
            ButtonState.Normal => _normalTween,
            ButtonState.Highlighted => _highlightedTween,
            ButtonState.Pressed => _pressedTween,
            _ => _normalTween,
        };
    }

    public enum ButtonState
    {
        None = 0,
        Normal = 1,
        Highlighted = 2,
        Pressed = 3,
    }
}
