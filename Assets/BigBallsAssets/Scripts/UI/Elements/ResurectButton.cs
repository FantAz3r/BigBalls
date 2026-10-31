using VContainer;

public class ResurectButton : AddButton
{
    private const string ResurectID = "resurect";
    private Resurecter _resurecter;

    [Inject]
    public void Construcr (Resurecter resurecter)
    {
        _resurecter = resurecter;
    }

    private void OnEnable ()
    {
        SetRewardAction(() => _resurecter.Resurect(), ResurectID);
    }
}
