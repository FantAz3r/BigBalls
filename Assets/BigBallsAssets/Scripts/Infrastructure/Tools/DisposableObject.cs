using System;

public class DisposableObject : IDisposable
{
    private readonly Action _action;
    private bool _disposed;
    public DisposableObject(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _action.Invoke();
    }
}
