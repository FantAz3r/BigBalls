using UnityEngine;

namespace BigBalls.Configs
{
    public class Cannon : MonoBehaviour
    {
        [field: SerializeField] public Transform FirePoint { get; private set; }
    }
}