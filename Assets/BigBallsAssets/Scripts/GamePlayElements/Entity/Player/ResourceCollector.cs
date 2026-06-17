using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Loot loot))
        {
            loot.Collect();
        }
    }
}
