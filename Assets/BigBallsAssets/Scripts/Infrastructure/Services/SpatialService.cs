using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using UnityEngine;

public class SpatialService : ISpatialService
{
    public IEnumerable<IEntity> GetEntitiesInRadius (Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IEntity>(out var entity))
            {
                yield return entity;
            }
        }
    }
}
