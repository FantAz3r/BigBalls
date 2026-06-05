using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if (other.TryGetComponent(out ResourcePiece resource))
        //{
        //    if (_activeResources.Contains(resource) == false)
        //    {
        //        resource.OnTake();
        //        _activeResources.Add(resource);
        //        StartCoroutine(RelocateResource(resource));
        //    }
        //}
    }
}
