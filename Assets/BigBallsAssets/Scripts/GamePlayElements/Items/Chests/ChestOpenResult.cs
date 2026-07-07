using System.Collections.Generic;

public class ChestOpenResult
{
    public List<ICardModel> Cards { get; private set; } = new List<ICardModel>();
    public int GoldAmount { get; set; }

    public bool HasCards => Cards.Count > 0;
    public bool HasGold => GoldAmount > 0;
    public int TotalItems => Cards.Count + (HasGold ? 1 : 0);
}