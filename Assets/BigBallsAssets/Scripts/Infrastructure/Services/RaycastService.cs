using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Services
{
    public class RaycastService : IRaycastService
    {
        public bool CheckCollisions(Transform origin, Vector3[] directions, Vector3[] points, float distance, LayerMask mask, out List<Vector3> hitNormals)
        {
            hitNormals = new List<Vector3>();
            bool hasCollision = false;

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 startPoint = points[i] + origin.position;
                Vector3 direction = directions[i].normalized;

                if (Physics.Raycast(startPoint, direction, out RaycastHit hit, distance, mask))
                {
                    hasCollision = true;
                    hitNormals.Add(hit.normal);
                    Debug.DrawRay(startPoint, direction * hit.distance, Color.red);
                }
                else
                {
                    Debug.DrawRay(startPoint, direction * distance, Color.green);
                }
            }

            return hasCollision;
        }

    }
}