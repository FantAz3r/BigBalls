using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSetter : MonoBehaviour
{
    private void OnEnable()
    {
        int position = Random.Range(0, 4);
        transform.localRotation = Quaternion.Euler(0, 90 * position, 0);
    }
}
