using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using UnityEngine;

public class FieldCell : MonoBehaviour
{
    public bool HasEnemy { get; private set; }
    public bool HasBall { get; private set; }

    private void OnTriggerEnter (Collider other)
    {
        IncludeSwitch(other, true);
    }

    private void OnTriggerExit (Collider other)
    {
        IncludeSwitch(other, false);
    }

    private void IncludeSwitch(Collider other, bool hasEntity)
    {
        if (other.TryGetComponent<Enemy>(out _))
        {
            HasEnemy = hasEntity;
        }
        else if (other.TryGetComponent<Ball>(out _))
        {
            HasBall = hasEntity;
        }
    }
}
