using System.Collections;
using UnityEngine;

public class TimeService : IService, ITimeService
{
    private readonly ICoroutineRunner _coroutineRunner;
    private Coroutine _slowMotionCoroutine;

    public bool IsPaused { get; private set; }

    public TimeService(ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void StopGame()
    {
        if(_slowMotionCoroutine != null)
            _coroutineRunner.StopCoroutine(_slowMotionCoroutine);

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void SmoothEditTimeScalse(float targetTimeScale, float duration)
    {
        if (_slowMotionCoroutine != null)
            _coroutineRunner.StopCoroutine(_slowMotionCoroutine);

        _slowMotionCoroutine = _coroutineRunner.StartCoroutine(EditRoutine(targetTimeScale, duration));
    }

    private IEnumerator EditRoutine(float targetTimeScale, float duration)
    {
        float startTimeScale = Time.timeScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; 
            Time.timeScale = Mathf.MoveTowards(startTimeScale, targetTimeScale, elapsed / duration);
            yield return null;
        }

        Time.timeScale = targetTimeScale;
        _slowMotionCoroutine = null;
    }
}


