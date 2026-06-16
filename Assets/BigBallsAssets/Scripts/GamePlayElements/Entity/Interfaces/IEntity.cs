using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public interface IEntity
    {
        int Id { get; }
        Transform Transform { get; }
        EntityEventHandler EventHandler { get; }
    }
}