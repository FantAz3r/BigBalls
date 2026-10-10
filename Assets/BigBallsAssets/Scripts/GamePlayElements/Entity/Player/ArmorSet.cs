using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSet : MonoBehaviour
{
    [SerializeField] private List<GameObject> _armorElements = new List<GameObject>();

    [field: SerializeField] public ItemType ItemType { get; private set; }
    public IEnumerable ArmorElements => _armorElements;
}
