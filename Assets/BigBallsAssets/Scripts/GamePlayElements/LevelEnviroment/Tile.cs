using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Tile : MonoBehaviour
    {
        private const int CenterLineIndex = 7;
        private const int AdditionalLines = 2;

        [SerializeField] private Wall _leftWall;
        [SerializeField] private Wall _rightWall;
        [SerializeField] private Ground _ground;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private float _scaleDuration = 1.0f;
        [SerializeField] private GameObject _water;
        [SerializeField] private List<TileLine> _tileLines = new();

        private int _currentWidth;
        private bool _isActionComplete = false;
        private bool _isScaling = false;
        private Tweener _scaleTweener;
        private int _halfWidth;


        public event Action Finished;

        private void Awake ()
        {
            _tileLines = GetComponentsInChildren<TileLine>(true).ToList();
        }

        public void Construct (int width)
        {
            _currentWidth = width + AdditionalLines;
            _halfWidth = _currentWidth / 2;
            ActivateLinesFromCenter(width);
        }

        public void ScaleTile (int newWidth)
        {
            if (_isScaling == false)
            {
                _currentWidth = newWidth;
                ScaleTileAnimation(newWidth);
            }
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.TryGetComponent<Player>(out _))
            {
                if (_isActionComplete == false)
                {
                    _isActionComplete = true;
                    Finished?.Invoke();
                }
            }
        }

        private void ActivateLinesFromCenter (int width)
        {
            foreach (var line in _tileLines)
                line.gameObject.SetActive(false);

            int startIndex = Mathf.Max(0, CenterLineIndex - _halfWidth);
            int endIndex = Mathf.Min(_tileLines.Count - 1, CenterLineIndex + _halfWidth);

            for (int i = startIndex; i <= endIndex; i++)
            {
                _tileLines[i].gameObject.SetActive(true);
            }
        }

        private void ScaleTileAnimation (int newWidth)
        {
            if (_scaleTweener != null && _scaleTweener.IsActive())
                _scaleTweener.Kill();

            _isScaling = true;
            Vector3 leftStart = _leftWall.transform.localPosition;
            Vector3 rightStart = _rightWall.transform.localPosition;
            Vector3 waterScale = _water.transform.localScale;

            Vector3 leftTarget = new Vector3(leftStart.x + 1, leftStart.y, leftStart.z);
            Vector3 rightTarget = new Vector3(rightStart.x - 1, rightStart.y, rightStart.z);
            Vector3 waterTargetScale = new Vector3(newWidth, _water.transform.localScale.y, _water.transform.localScale.z);

            _scaleTweener = DOTween.To(
                () => 0f,
                t =>
                {
                    float progress = Mathf.SmoothStep(0, 1, t);
                    _leftWall.transform.localPosition = Vector3.Lerp(leftStart, leftTarget, progress);
                    _rightWall.transform.localPosition = Vector3.Lerp(rightStart, rightTarget, progress);
                    _water.transform.localScale = Vector3.Lerp(waterScale, waterTargetScale, progress);
                },
                1f,
                _scaleDuration
            ).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                ActivateExtraLines(newWidth);
                SetScale(leftTarget, rightTarget);

                _isScaling = false;
                _scaleTweener = null;
            });
        }

        private void ActivateExtraLines (int width)
        {
            int leftExtraIndex = Mathf.Max(0, CenterLineIndex - _halfWidth - 1);
            int rightExtraIndex = Mathf.Min(_tileLines.Count - 1, CenterLineIndex + _halfWidth + 1);

            if (leftExtraIndex >= 0 && leftExtraIndex < _tileLines.Count)
                _tileLines[leftExtraIndex].gameObject.SetActive(true);

            if (rightExtraIndex >= 0 && rightExtraIndex < _tileLines.Count)
                _tileLines[rightExtraIndex].gameObject.SetActive(true);
        }

        private void SetScale (Vector3 leftWallPosition, Vector3 rightWallPosition)
        {
            _leftWall.transform.localPosition = leftWallPosition;
            _rightWall.transform.localPosition = rightWallPosition;
        }

        private void OnDisable ()
        {
            if (_scaleTweener != null && _scaleTweener.IsActive())
                _scaleTweener.Kill();
        }
    }
}
