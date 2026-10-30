using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ITriggerCollider 
{
    void OnTriggerEnter ();
    void OnTriggerExit ();
}
