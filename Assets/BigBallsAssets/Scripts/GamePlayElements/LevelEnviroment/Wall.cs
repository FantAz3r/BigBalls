using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private List<Transform> _walls;
        [SerializeField] private List<Transform> _column;

        public void BuildWall(GameObject wall, GameObject column)
        {
            foreach (Transform t in _walls)
            {
                Instantiate(wall, t);
            }

            if (column == null)
                return;

            foreach (Transform t in _column)
            {
                Instantiate(column, t);
            }
        }    
    }
}