using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private bool _useSound = true;
    [SerializeField] private ButtonStateSettings _normal;
    [SerializeField] private ButtonStateSettings _highlighted;
    [SerializeField] private ButtonStateSettings _pressed;

    private Image _targetImage;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private RectTransformSnapshot _originRect;
    private CanvasGroupSnapshot _originCanvasGroup;
    private SoundSpawner _soundSpawner;

    private ButtonState _currentState;
    private Tween _normalTween;
    private Tween _highlightedTween;
    private Tween _pressedTween;

    private CachedSettings _cachedNormal;
    private CachedSettings _cachedHighlighted;
    private CachedSettings _cachedPressed;
    private bool _isInitialized;
    private bool _isPointerOver;

    private struct CachedSettings
    {
        public Sprite Sprite;
        public Color Color;
        public float Duration;
        public Ease Ease;
        public float Delay;
        public bool EnabledPositionChange;
        public bool EnabledCanvasGroupChange;
        public bool InteractableWhenPlay;
        public bool BlockRaycastWhenPlay;
        public bool IgnoreParentGroupWhenPlay;
        public bool UseScale;
        public Vector3 TargetScale;
        public bool UsePosition;
        public Vector2 AnchoredOffset;
        public bool UseRotation;
        public Vector3 TargetRotation;
        public bool UseFade;
        public float TargetAlpha;

        public CachedSettings (ButtonStateSettings settings)
        {
            Sprite = settings.Sprite;
            Color = settings.Color;

            var anim = settings.AnimationSettings;
            Duration = anim.Duration;
            Ease = anim.Ease;
            Delay = anim.Delay;
            EnabledPositionChange = anim.EnabledPositionChange;
            EnabledCanvasGroupChange = anim.EnabledCanvasGroupChange;
            InteractableWhenPlay = anim.InteractableWhenPlay;
            BlockRaycastWhenPlay = anim.BlockRaycastWhenPlay;
            IgnoreParentGroupWhenPlay = anim.IgnoreParentGroupWhenPlay;

            UseScale = anim.TryGetTargetScale(out var scale);
            TargetScale = scale;

            UsePosition = anim.TryGetAnchoredOffset(out var offset);
            AnchoredOffset = offset;

            UseRotation = anim.TryGetTargetRotation(out var rotation);
            TargetRotation = rotation;

            UseFade = anim.TryGetTargetAlpha(out var alpha);
            TargetAlpha = alpha;
        }

        public bool IsEqual (ButtonStateSettings settings)
        {
            if (settings == null) return false;
            if (settings.Sprite != Sprite) return false;
            if (settings.Color != Color) return false;

            var anim = settings.AnimationSettings;
            if (anim.Duration != Duration) return false;
            if (anim.Ease != Ease) return false;
            if (anim.Delay != Delay) return false;
            if (anim.EnabledPositionChange != EnabledPositionChange) return false;
            if (anim.EnabledCanvasGroupChange != EnabledCanvasGroupChange) return false;
            if (anim.InteractableWhenPlay != InteractableWhenPlay) return false;
            if (anim.BlockRaycastWhenPlay != BlockRaycastWhenPlay) return false;
            if (anim.IgnoreParentGroupWhenPlay != IgnoreParentGroupWhenPlay) return false;

            if (anim.TryGetTargetScale(out var scale) != UseScale) return false;
            if (UseScale && scale != TargetScale) return false;

            if (anim.TryGetAnchoredOffset(out var offset) != UsePosition) return false;
            if (UsePosition && offset != AnchoredOffset) return false;

            if (anim.TryGetTargetRotation(out var rotation) != UseRotation) return false;
            if (UseRotation && rotation != TargetRotation) return false;

            if (anim.TryGetTargetAlpha(out var alpha) != UseFade) return false;
            if (UseFade && !Mathf.Approximately(alpha, TargetAlpha)) return false;

            return true;
        }

        public void Update (ButtonStateSettings settings)
        {
            Sprite = settings.Sprite;
            Color = settings.Color;

            var anim = settings.AnimationSettings;
            Duration = anim.Duration;
            Ease = anim.Ease;
            Delay = anim.Delay;
            EnabledPositionChange = anim.EnabledPositionChange;
            EnabledCanvasGroupChange = anim.EnabledCanvasGroupChange;
            InteractableWhenPlay = anim.InteractableWhenPlay;
            BlockRaycastWhenPlay = anim.BlockRaycastWhenPlay;
            IgnoreParentGroupWhenPlay = anim.IgnoreParentGroupWhenPlay;

            UseScale = anim.TryGetTargetScale(out var scale);
            TargetScale = scale;

            UsePosition = anim.TryGetAnchoredOffset(out var offset);
            AnchoredOffset = offset;

            UseRotation = anim.TryGetTargetRotation(out var rotation);
            TargetRotation = rotation;

            UseFade = anim.TryGetTargetAlpha(out var alpha);
            TargetAlpha = alpha;
        }
    }

    [Inject]
    public void Construct(SoundSpawner soundSpawner)
    {
        _soundSpawner = soundSpawner;
        _targetImage = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _originRect = new RectTransformSnapshot(_rectTransform);
        _originCanvasGroup = new CanvasGroupSnapshot(_canvasGroup);

    }

    private void Start ()
    {
        InitializeAnimations();
        _isInitialized = true;
    }

    //private void Update ()
    //{
    //    if (!_isInitialized)
    //        return;
    //
    //    CheckAndUpdateAnimations();
    //}

    private void OnDisable ()
    {
        TrySetState(ButtonState.Normal, instant: true);
    }

    private void OnDestroy ()
    {
        _normalTween?.Kill();
        _highlightedTween?.Kill();
        _pressedTween?.Kill();
    }

   // private void OnValidate ()
   // {
   //     if (Application.isPlaying && _isInitialized)
   //     {
   //         CheckAndUpdateAnimations();
   //     }
   // }

    public void OnPointerEnter (PointerEventData eventData)
    {
        _isPointerOver = true;
        TrySetState(ButtonState.Highlighted);

        if(_useSound)
            _soundSpawner.Spawn(SoundType.ButtonSelected);
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        _isPointerOver = false;
        TrySetState(ButtonState.Normal);
    }


    public void OnPointerClick (PointerEventData eventData)
    {
        SetState(ButtonState.Pressed);

        if (_useSound)
            _soundSpawner.Spawn(SoundType.ButtonClick);
    }

    private void InitializeAnimations ()
    {
        _normalTween = ConstructTween(_normal);
        _highlightedTween = ConstructTween(_highlighted);
        _pressedTween = ConstructTween(_pressed, OnPressedComplete);

        SetState(ButtonState.Normal, instant: true);

        _cachedNormal = new CachedSettings(_normal);
        _cachedHighlighted = new CachedSettings(_highlighted);
        _cachedPressed = new CachedSettings(_pressed);
    }

    private void CheckAndUpdateAnimations ()
    {
        bool needUpdate = false;

        if (!_cachedNormal.IsEqual(_normal))
        {
            RebuildAnimation(ButtonState.Normal, _normal, ref _normalTween);
            _cachedNormal.Update(_normal);
            needUpdate = true;
        }

        if (!_cachedHighlighted.IsEqual(_highlighted))
        {
            RebuildAnimation(ButtonState.Highlighted, _highlighted, ref _highlightedTween);
            _cachedHighlighted.Update(_highlighted);
            needUpdate = true;
        }

        if (!_cachedPressed.IsEqual(_pressed))
        {
            RebuildAnimation(ButtonState.Pressed, _pressed, ref _pressedTween);
            _cachedPressed.Update(_pressed);
            needUpdate = true;
        }

        if (needUpdate && _currentState != ButtonState.None)
        {
            Tween currentAnimation = GetStateAnimation(_currentState);

            if (currentAnimation != null)
            {
                currentAnimation.Restart();
            }
        }
    }

    private void RebuildAnimation (ButtonState state, ButtonStateSettings settings, ref Tween currentTween)
    {
        currentTween?.Kill();

        if (state == ButtonState.Pressed)
        {
            currentTween = ConstructTween(settings, OnPressedComplete);
        }
        else
        {
            currentTween = ConstructTween(settings);
        }
    }

    private void TrySetState (ButtonState newState, bool instant = false)
    {
        if (newState == _currentState)
        {
            return;
        }

        Tween currentAnimation = GetStateAnimation(_currentState);

        if (currentAnimation != null && !currentAnimation.IsComplete() && _currentState == ButtonState.Pressed)
        {
            return;
        }

        SetState(newState, instant);
    }

    private void SetState (ButtonState newState, bool instant = false)
    {
        Tween currentAnimation = GetStateAnimation(_currentState);

        if (currentAnimation != null && !currentAnimation.IsComplete())
        {
            currentAnimation.Complete();
        }

        _currentState = newState;
        currentAnimation = GetStateAnimation(_currentState);
        currentAnimation?.Restart();

        if (instant && currentAnimation != null)
        {
            currentAnimation.Complete(withCallbacks: true);
        }
    }

    private Tween ConstructTween (ButtonStateSettings settings, Action onComplete = null)
    {
        Sequence sequence = DOTween.Sequence();

        Vector3 targetScale = settings.AnimationSettings.TryGetTargetScale(out var scale) ? scale : _originRect.Scale;
        Vector2 anchoredOffset = settings.AnimationSettings.TryGetAnchoredOffset(out var offset) ? offset : Vector2.zero;
        Vector3 targetRotation = settings.AnimationSettings.TryGetTargetRotation(out var rotation) ? rotation : _originRect.Rotation;
        float targetAlpha = settings.AnimationSettings.TryGetTargetAlpha(out var alpha) ? alpha : _originCanvasGroup.Alpha;

        float duration = settings.AnimationSettings.Duration;
        Ease ease = settings.AnimationSettings.Ease;
        float delay = settings.AnimationSettings.Delay;

        sequence.Append(_rectTransform.DOScale(targetScale, duration).SetEase(ease));

        if (settings.AnimationSettings.EnabledPositionChange)
        {
            sequence.Join(_rectTransform.DOAnchorPos(_originRect.AnchoredPosition + anchoredOffset, duration).SetEase(ease));
        }

        sequence.Join(_rectTransform.DORotate(targetRotation, duration).SetEase(ease));

        if (settings.AnimationSettings.EnabledCanvasGroupChange)
        {
            sequence.Join(_canvasGroup.DOFade(targetAlpha, duration).SetEase(ease));
        }

        if (_targetImage != null)
        {
            sequence.Join(_targetImage.DOColor(settings.Color, duration).SetEase(ease));
        }

        sequence.SetDelay(delay)
            .SetAutoKill(false)
            .SetUpdate(true)
            .OnPlay(() =>
            {
                if (settings.AnimationSettings.EnabledCanvasGroupChange)
                {
                    _canvasGroup.interactable = settings.AnimationSettings.InteractableWhenPlay;
                    _canvasGroup.blocksRaycasts = settings.AnimationSettings.BlockRaycastWhenPlay;
                    _canvasGroup.ignoreParentGroups = settings.AnimationSettings.IgnoreParentGroupWhenPlay;
                }
            })
            .OnComplete(() =>
            {
                if (_targetImage != null)
                {
                    _targetImage.sprite = settings.Sprite;
                }

                if (settings.AnimationSettings.EnabledCanvasGroupChange)
                {
                    _canvasGroup.interactable = _originCanvasGroup.Interactable;
                    _canvasGroup.blocksRaycasts = _originCanvasGroup.BlocksRaycasts;
                    _canvasGroup.ignoreParentGroups = _originCanvasGroup.IgnoreParentGroups;
                }

                onComplete?.Invoke();
            });

        sequence.Rewind();

        return sequence;
    }


    private void OnPressedComplete ()
    {
        SetState(_isPointerOver ? ButtonState.Highlighted : ButtonState.Normal);
    }

    private Tween GetStateAnimation (ButtonState state)
    {
        return state switch
        {
            ButtonState.Normal => _normalTween,
            ButtonState.Highlighted => _highlightedTween,
            ButtonState.Pressed => _pressedTween,
            _ => _normalTween,
        };
    }

    public void RefreshAnimations ()
    {
        if (!_isInitialized)
            return;

        CheckAndUpdateAnimations();
    }

    public enum ButtonState
    {
        None = 0,
        Normal = 1,
        Highlighted = 2,
        Pressed = 3,
    }
}