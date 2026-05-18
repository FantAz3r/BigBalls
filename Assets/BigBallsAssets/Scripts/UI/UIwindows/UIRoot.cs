using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [field: SerializeField] public Transform SafeAreaUIHolder { get; private set; }
    [field: SerializeField] public Transform BackgroundUIHolder { get; private set; }

    private void OnDestroy()
    {
        Debug.Log("123");
    }
}
