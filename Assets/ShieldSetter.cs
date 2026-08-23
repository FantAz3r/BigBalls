using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSetter : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(0, 90 * Random.Range(0f, 3f), 0);
    }
}
