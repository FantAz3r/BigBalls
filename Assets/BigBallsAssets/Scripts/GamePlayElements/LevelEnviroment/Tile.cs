using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private const int CenterLineIndex = 7;
    private const int AdditionalLines = 2;
    private const float LineWidth = 1f; 

    [SerializeField] private Wall _leftWall;
    [SerializeField] private Wall _rightWall;
    [SerializeField] private Ground _ground;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private float _scaleDuration = 1.0f;
    [SerializeField] private List<TileLine> _tileLines = new();

    private int _halfWidth;
    private int _currentWidth;
    private bool _isActionComplete = false;
    private bool _isScaling = false;
    private Tweener _scaleTweener;

    private Vector3 _leftWallBasePos = new Vector3(7, 0, 7);
    private Vector3 _rightWallBasePos = new Vector3(-1, 0, 7);

    public event Action Finished;

    private void Awake ()
    {
        _tileLines = GetComponentsInChildren<TileLine>(true).ToList();
    }

    public void Init (int width)
    {
        _currentWidth = width + AdditionalLines;
        _halfWidth = _currentWidth / 2;
        ActivateLines(_halfWidth);
        ApplyWallsForHalfWidth(_halfWidth);
    }

    public void ScaleTile (int newWidth)
    {
        int newCurrentWidth = newWidth + AdditionalLines;
        int newHalfWidth = newCurrentWidth / 2;

        if (newHalfWidth == _halfWidth && _isScaling == false)
            return;

        _currentWidth = newCurrentWidth;
        _halfWidth = newHalfWidth;

        ActivateLines(_halfWidth);
        AnimateWallsTo(_halfWidth);
    }

    private void ActivateLines (int halfWidth)
    {
        foreach (var line in _tileLines)
            line.gameObject.SetActive(false);

        int startIndex = Mathf.Max(0, CenterLineIndex - halfWidth);
        int endIndex = Mathf.Min(_tileLines.Count - 1, CenterLineIndex + halfWidth);

        for (int i = startIndex; i <= endIndex; i++)
            _tileLines[i].gameObject.SetActive(true);
    }

    private void ApplyWallsForHalfWidth (int halfWidth)
    {
        float offset = halfWidth * LineWidth;
        _leftWall.transform.localPosition = _leftWallBasePos + new Vector3(offset, 0f, 0f);
        _rightWall.transform.localPosition = _rightWallBasePos + new Vector3(-offset, 0f, 0f);
    }

    private void AnimateWallsTo (int halfWidth)
    {
        if (_scaleTweener != null && _scaleTweener.IsActive())
            _scaleTweener.Kill();

        _isScaling = true;

        Vector3 leftStart = _leftWall.transform.localPosition;
        Vector3 rightStart = _rightWall.transform.localPosition;

        float offset = halfWidth * LineWidth;
        Vector3 leftTarget = _leftWallBasePos + new Vector3(offset, 0f, 0f);
        Vector3 rightTarget = _rightWallBasePos + new Vector3(-offset, 0f, 0f);

        _scaleTweener = DOTween.To(
            () => 0f,
            t =>
            {
                float progress = Mathf.SmoothStep(0f, 1f, t);
                _leftWall.transform.localPosition = Vector3.Lerp(leftStart, leftTarget, progress);
                _rightWall.transform.localPosition = Vector3.Lerp(rightStart, rightTarget, progress);
            },
            1f,
            _scaleDuration
        ).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _leftWall.transform.localPosition = leftTarget;
            _rightWall.transform.localPosition = rightTarget;
            _isScaling = false;
            _scaleTweener = null;
        });
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.TryGetComponent<Player>(out _) && !_isActionComplete)
        {
            _isActionComplete = true;
            Finished?.Invoke();
        }
    }

    private void OnDisable ()
    {
        if (_scaleTweener != null && _scaleTweener.IsActive())
            _scaleTweener.Kill();
        _scaleTweener = null;
        _isScaling = false;
        _isActionComplete = false;
    }

    public void SetLines (List<TileLine> tileLines) => _tileLines = tileLines;
}