using BigBalls.Services;
using System;
using System.Collections;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Wall _leftWall;
        [SerializeField] private Wall _rightWall;
        [SerializeField] private Ground _ground;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private float _scaleDuration = 1.0f;

        private ICoroutineRunner _coroutineRunner;
        private int _currentWidth;
        private bool _isActionComplite = false;
        private bool _isScaling = false;

        public event Action Finished;

        public void Construct(ICoroutineRunner coroutineRunner, int width)
        {
            _coroutineRunner = coroutineRunner;
            _currentWidth = width;
            SetScale(width, width, width);
        }

        public void ScaleTile(int newWidth)
        {
            if (_isScaling == false)
            {
                _currentWidth = newWidth;
                _coroutineRunner.StartCoroutine(ScaleTileRoutine());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out _))
            {
                if (_isActionComplite == false)
                {
                    _isActionComplite = true;
                    Finished?.Invoke();
                }
            }
        }

        private void SetScale(float width, float groundScaleX, float colliderSizeX)
        {
            float halfWidth = width / 2f;

            _leftWall.transform.localPosition = new Vector3(-halfWidth, _leftWall.transform.localPosition.y, _leftWall.transform.localPosition.z);
            _rightWall.transform.localPosition = new Vector3(halfWidth, _rightWall.transform.localPosition.y, _rightWall.transform.localPosition.z);

            _ground.transform.localScale = new Vector3(groundScaleX, _ground.transform.localScale.y, _ground.transform.localScale.z);

            _boxCollider.size = new Vector3(colliderSizeX, _boxCollider.size.y, _boxCollider.size.z);
            _boxCollider.center = new Vector3(0, _boxCollider.center.y, _boxCollider.center.z);
        }

        private IEnumerator ScaleTileRoutine()
        {
            _isScaling = true;
            float elapsed = 0;
            float targetHalfWidth = _currentWidth / 2f;

            Vector3 leftStart = _leftWall.transform.localPosition;
            Vector3 rightStart = _rightWall.transform.localPosition;
            Vector3 groundStart = _ground.transform.localScale;
            Vector3 boxStart = _boxCollider.size;

            Vector3 leftTarget = new Vector3(-targetHalfWidth, leftStart.y, leftStart.z);
            Vector3 rightTarget = new Vector3(targetHalfWidth, rightStart.y, rightStart.z);
            Vector3 groundTarget = new Vector3(_currentWidth, groundStart.y, groundStart.z); 
            Vector3 boxTarget = new Vector3(_currentWidth, boxStart.y, boxStart.z);


            while (elapsed < _scaleDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / _scaleDuration);

                _leftWall.transform.localPosition = Vector3.Lerp(leftStart, leftTarget, t);
                _rightWall.transform.localPosition = Vector3.Lerp(rightStart, rightTarget, t);
                _ground.transform.localScale = Vector3.Lerp(groundStart, groundTarget, t);
                _boxCollider.size = Vector3.Lerp(boxStart, boxTarget, t);

                yield return null;
            }

            SetScale(_currentWidth, groundTarget.x, boxTarget.x);
            _isScaling = false;
        }
    }
}