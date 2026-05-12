using UnityEngine;

public class GameRunner : MonoBehaviour
{
    [SerializeField] private EntryPoint _entyPoint;

    private void Awake()
    {
        EntryPoint entyPoint = FindFirstObjectByType<EntryPoint>();

        if (entyPoint == null)
            _entyPoint = Instantiate(_entyPoint);
    }
}
