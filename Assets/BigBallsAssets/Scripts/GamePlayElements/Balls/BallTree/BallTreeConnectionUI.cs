using System;
using System.Collections;
using System.Collections.Generic;
using UI_Spline_Renderer;
using UnityEngine;

public class BallTreeConnectionUI : MonoBehaviour
{
    private UISplineRenderer _splineRenderer;

    [Header("Visual Settings")]
    [SerializeField] private Material _material;
    [SerializeField] private Texture _texture;
    [SerializeField] private float _lineWidth = 10f;
    [SerializeField] private Color _lockedColor = Color.gray;
    [SerializeField] private Color _unlockedColor = Color.green;
    [SerializeField] private Color _upgradedColor = Color.green;

    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private Coroutine _fillCoroutine;

    public bool IsConnected { get; private set; }

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
        _splineRenderer.clipRange = new Vector2(0, 0);

        _splineRenderer.material = _material;
        _splineRenderer.uvMultiplier = new Vector2(1, -1);
        _splineRenderer.texture = _texture;

        _splineRenderer.color = _lockedColor;
    }

    public void UpdateLine ()
    {
        gameObject.SetActive(true);

        if (IsConnected)
            return;
        _fillCoroutine = StartCoroutine(FillRoutine());
    }

    public void UpgradeConnaction()
    {
        _splineRenderer.color = _upgradedColor;
    }

    public void DisableLine ()
    {
        gameObject.SetActive(false);
        IsConnected = false;

        if (_fillCoroutine != null)
            StopCoroutine(_fillCoroutine);
    }

    private IEnumerator FillRoutine ()
    {
        _splineRenderer.color = _lockedColor;
        float elapsed = 0f;

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _animationDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            _splineRenderer.clipRange = new Vector2(0, smoothT);

            yield return null;
        }

        IsConnected = true;
        _splineRenderer.clipRange = new Vector2(0, 1);
        _splineRenderer.color = _unlockedColor;
    }
}
