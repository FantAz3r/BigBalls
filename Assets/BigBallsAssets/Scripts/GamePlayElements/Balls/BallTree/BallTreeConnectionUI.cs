using UI_Spline_Renderer;
using UnityEngine;
using System.Collections.Generic;

public class BallTreeConnectionUI : MonoBehaviour
{
    private UISplineRenderer _splineRenderer;

    [Header("Visual Settings")]
    [SerializeField] private float _lineWidth = 10f;
    [SerializeField] private Color _lockedColor = Color.gray;
    [SerializeField] private Color _unlockedColor = Color.green;

    private bool _isUnlocked;

    public bool IsUnlocked => _isUnlocked;

    private void Awake ()
    {
        transform.SetAsFirstSibling();
    }

    public void SetConnection (RectTransform from, RectTransform to)
    {
        var positions = new List<Vector3>
        {
            new Vector3(from.localPosition.x, from.localPosition.y, 0),
            new Vector3(to.localPosition.x, to.localPosition.y, 0)
        };

        _splineRenderer = UISplineRenderer.Create(
            positions,
            transform as RectTransform,
            true,
            LineTexturePreset.Default,
            StartEndImagePreset.None,
            StartEndImagePreset.None
        );

        _splineRenderer.transform.SetParent(transform);
        _splineRenderer.transform.localPosition = Vector3.zero;
        _splineRenderer.width = _lineWidth;
        _splineRenderer.color = _isUnlocked ? _unlockedColor : _lockedColor;
    }

    public void UpdateLine()
    {
        _splineRenderer.color = _isUnlocked ? _unlockedColor : _lockedColor;
    }
}