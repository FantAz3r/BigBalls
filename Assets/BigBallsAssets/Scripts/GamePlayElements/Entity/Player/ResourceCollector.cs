using System.Collections;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    private float _moveSpeed = 9f;
    private float _arrivalThreshold = 0.3f;

    private void OnTriggerEnter (Collider other)
    {
        if (other.TryGetComponent(out Loot loot))
        {
            StartCoroutine(RelocateCoroutine(loot));
        }
    }

    private IEnumerator RelocateCoroutine (Loot loot)
    {
        loot.Collider.enabled = false;

        while (Vector3.SqrMagnitude(loot.transform.position - transform.position) > _arrivalThreshold * _arrivalThreshold)
        {
            loot.transform.position = Vector3.Lerp(
                loot.transform.position,
                transform.position,
                _moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        loot.transform.position = transform.position;
        loot.Collect();
        loot.Collider.enabled = true;
    }
}
