using BigBalls.UI;
using VContainer;

public class ResetProgressButton : ButtonClickHandler
{
    private ISaveService _saveService;

    [Inject]
    public void Construct (ISaveService saveService)
    {
        _saveService = saveService;
    }

    protected override void OnClick ()
    {
        _saveService.ResetSave();
    }
}
