public interface ILootFactory
{
    void Disable();
    Loot Create(string name);
}