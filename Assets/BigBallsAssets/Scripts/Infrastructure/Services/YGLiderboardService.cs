using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.Services;
using UnityEngine;
using YG;

public class YGLiderboardService : ILiderboardService
{
    private const float ThrottleInterval = 2.0f;

    private readonly ICoroutineRunner _coroutineRunner;
    private readonly Queue<Action> _requestQueue = new();

    private bool _isProcessing;
    private float _lastRequestTime = -ThrottleInterval;
    private Coroutine _workerCoroutine;

    public YGLiderboardService (ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public event Action<string, int> OnScoreSubmitted;
    public void SubmitScore (string leaderboardName, int score)
    {
        _requestQueue.Enqueue(() =>
        {
            YG2.SetLeaderboard(leaderboardName, score);
            OnScoreSubmitted?.Invoke(leaderboardName, score);
        });

        if (!_isProcessing)
        {
            _isProcessing = true;
            _workerCoroutine = _coroutineRunner.StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue ()
    {
        while (_requestQueue.Count > 0)
        {
            float timeSinceLast = Time.realtimeSinceStartup - _lastRequestTime;

            if (timeSinceLast < ThrottleInterval)
            {
                float waitTime = ThrottleInterval - timeSinceLast;
                yield return new WaitForSecondsRealtime(waitTime);
            }

            Action request = _requestQueue.Dequeue();

            try
            {
                request?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[YGLiderboardService] Ошибка при выполнении запроса: {ex.Message}");
            }

            _lastRequestTime = Time.realtimeSinceStartup;
        }

        _isProcessing = false;
        _workerCoroutine = null;
    }

    public void Dispose ()
    {
        if (_workerCoroutine != null && _coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_workerCoroutine);
            _workerCoroutine = null;
        }
        _requestQueue.Clear();
        _isProcessing = false;
    }
}