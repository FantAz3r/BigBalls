using UnityEngine;

public class ResourceCollector : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Loot loot))
        {
            Relocate(loot);
            loot.Collect();
        }
    }

    private void Relocate(Loot loot)
    {

    }
}
