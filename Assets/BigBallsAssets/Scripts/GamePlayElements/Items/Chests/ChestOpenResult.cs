using System.Collections.Generic;
using BigBalls.StaticData;
// Новая модель результата открытия сундука
public class ChestOpenResult
{
    public List<ItemModel> Cards { get; set; } = new List<ItemModel>();
    public int GoldAmount { get; set; } = 0;

    public bool HasCards => Cards.Count > 0;
    public bool HasGold => GoldAmount > 0;
}