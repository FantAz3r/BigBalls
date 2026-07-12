using UnityEngine;

public struct RectTransformSnapshot
    {
        public RectTransformSnapshot(RectTransform rectTransform)
        {
            AnchoredPosition = rectTransform.anchoredPosition;
            Scale = rectTransform.localScale;
            Rotation = rectTransform.localEulerAngles;
        }

        public Vector2 AnchoredPosition { get; }
        public Vector3 Scale { get; }
        public Vector3 Rotation { get; }

        public override string ToString()
        {
            return $"{nameof(RectTransform)}: {nameof(AnchoredPosition)}({AnchoredPosition}), {nameof(Scale)}({Scale}), {nameof(Rotation)}({Rotation})";
        }
    }
