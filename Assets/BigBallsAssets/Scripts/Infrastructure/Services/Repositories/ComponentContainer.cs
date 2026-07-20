using BigBalls.GameplayObjects;
using System;
using System.Collections.Generic;

public class ComponentContainer
{
    private Dictionary<Type, ISubscribable> _components = new();

    public T Get<T>() where T : ISubscribable
    {
        return (T)_components[typeof(T)];
    }

    public void Add<T>(T component) where T : ISubscribable
    {
        _components[typeof(T)] = component;
    }

    public void Add (ISubscribable subscribable)
    {
        _components[subscribable.GetType()] = subscribable;
    }
}