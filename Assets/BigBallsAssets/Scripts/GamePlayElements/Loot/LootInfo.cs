using System;
using UnityEngine;

[Serializable]
public class LootInfo
{
    [Tooltip("Префаб лута")] 
    public LootType LootType;
    
    [Tooltip("Шанс выпадения (0-1). Например 0.3 = 30%")]
    [Range(0f, 1f)]
    public float DropChance = 0.5f;
    
    [Tooltip("Минимальное количество")]
    public int MinAmount = 1;
    
    [Tooltip("Максимальное количество")]
    public int MaxAmount = 3;
}