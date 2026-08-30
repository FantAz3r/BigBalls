public interface ILootFactory
{
    void Disable();
    Loot Create(Loot prefab);
}