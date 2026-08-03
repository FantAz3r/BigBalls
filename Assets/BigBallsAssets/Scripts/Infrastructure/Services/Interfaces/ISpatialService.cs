using System.Collections.Generic;
using BigBalls.GameplayObjects;
using UnityEngine;

public interface ISpatialService
{
    IEnumerable<IEntity> GetEntitiesInRadius (Vector3 position, float radius);
}
