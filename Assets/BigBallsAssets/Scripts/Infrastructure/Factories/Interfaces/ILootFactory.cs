using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILootFactory
{
    Loot Create(LootInfo lootInfo);
}
