using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IRaycastService
    {
        bool CheckCollisions(Transform origin, Vector3[] directions, Vector3[] points, float distance, LayerMask mask, out List<Vector3> hitNormals);
    }
}