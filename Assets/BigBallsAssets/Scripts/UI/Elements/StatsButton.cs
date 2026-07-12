using BigBalls.UI;

public class StatsButton : ButtonClickHandler
{
    private UIItem _item;
<<<<<<< HEAD
    private InventoryStatsView _stats;
=======
    private StatsView _stats;
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf

    private void Awake ()
    {
        _item = GetComponent<UIItem>();
    }

<<<<<<< HEAD
    public void Init(InventoryStatsView statsView)
=======
    public void Init(StatsView statsView)
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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
