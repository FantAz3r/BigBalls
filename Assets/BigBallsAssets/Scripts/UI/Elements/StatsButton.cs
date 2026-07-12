using BigBalls.UI;

public class StatsButton : ButtonClickHandler
{
    private UIItem _item;
    private InventoryStatsView _stats;

    private void Awake ()
    {
        _item = GetComponent<UIItem>();
    }

    public void Init(InventoryStatsView statsView)
    {
        _stats = statsView;
    }

    public void View () => OnClick();


    protected override void OnClick ()
    {
        base.OnClick();
        _stats.View(_item.Model);
    }
}
