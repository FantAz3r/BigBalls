using System.Collections.Generic;
using BigBalls.StaticData;

public class Inventory 
{
    private List<ItemModel> _items = new ();

    public void Add(ItemModel item)
    {
        _items.Add(item);
    }

    public void Remove(ItemModel item)
    {
        _items.Remove(item);
    }
}
