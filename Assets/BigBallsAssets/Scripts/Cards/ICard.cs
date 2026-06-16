using UnityEngine;

public interface ICard
{
    Sprite Icon { get; }
    string Name { get; }
    string Description { get; }
    int Level { get; }

    void Upgrade();
}