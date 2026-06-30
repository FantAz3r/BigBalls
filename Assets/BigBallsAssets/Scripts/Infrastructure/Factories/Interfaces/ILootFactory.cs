public interface ILootFactory
{
    void Dispose();
    Loot Create(string name);
}