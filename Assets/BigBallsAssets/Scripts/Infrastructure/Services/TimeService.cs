using System.Collections;
using UnityEngine;

namespace BigBalls.Services
{
    public class TimeService : IService, ITimeService
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private Coroutine _slowMotionCoroutine;
        private float _currentTimescale = 1;
        public bool IsPaused { get; private set; }

        public TimeService (ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void StopGame ()
        {
            if (_slowMotionCoroutine != null)
                _coroutineRunner.StopCoroutine(_slowMotionCoroutine);

            Time.timeScale = 0;
            IsPaused = true;

            Debug.Log($"{_currentTimescale}  {IsPaused}");

        }

        public void ResumeGame ()
        {
            Time.timeScale = _currentTimescale;
            IsPaused = false;
        }

        public void SetTimescale (float value)
        {
            _currentTimescale = Mathf.Max(value, 0.1f);
            Time.timeScale = _currentTimescale;
            IsPaused = false;
        }

        public void SmoothRecoverTimescale (float duration)
        {
            if (_slowMotionCoroutine != null)
                _coroutineRunner.StopCoroutine(_slowMotionCoroutine);

            _slowMotionCoroutine = _coroutineRunner.StartCoroutine(EditRoutine(_currentTimescale, duration));
        }

        public void SmoothEditTimescale (float targetTimescale, float duration)
        {
            if (_slowMotionCoroutine != null)
                _coroutineRunner.StopCoroutine(_slowMotionCoroutine);

            _slowMotionCoroutine = _coroutineRunner.StartCoroutine(EditRoutine(targetTimescale, duration));
        }


        private IEnumerator EditRoutine (float targetTimeScale, float duration)
        {
            float startTimeScale = Time.timeScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.MoveTowards(startTimeScale, targetTimeScale, elapsed / duration);
                yield return null;
            }

            if (targetTimeScale > 0)
            {
                _currentTimescale = targetTimeScale;
                IsPaused = false;
            }

            Time.timeScale = targetTimeScale;
            _slowMotionCoroutine = null;
        }
    }
}


