using UnityEngine;

public class LevelTimeCounter
{
    private float _startRealtime;
    private float _accumulated;
    private bool _isRunning;

    public bool IsRunning => _isRunning;

    public float Elapsed
    {
        get
        {
            if (_isRunning == false)
                return _accumulated;

            return _accumulated + (Time.realtimeSinceStartup - _startRealtime);
        }
    }

    public string ElapsedFormatted => Format(Elapsed);

    public void Start ()
    {
        _accumulated = 0f;
        _startRealtime = Time.realtimeSinceStartup;
        _isRunning = true;
    }

    public void Stop ()
    {
        if (_isRunning == false)
            return;

        _accumulated += Time.realtimeSinceStartup - _startRealtime;
        _isRunning = false;
    }

    public void Continue ()
    {
        if (_isRunning)
            return;

        _startRealtime = Time.realtimeSinceStartup;
        _isRunning = true;
    }

    public void Reset ()
    {
        _accumulated = 0f;
        _startRealtime = 0f;
        _isRunning = false;
    }

    private static string Format (float seconds)
    {
        if (seconds < 0f)
            seconds = 0f;

        int total = (int) seconds;
        int h = total / 3600;
        int m = (total % 3600) / 60;
        int s = total % 60;

        return h > 0 ? $"{h:00}:{m:00}:{s:00}" : $"{m:00}:{s:00}";
    }
}