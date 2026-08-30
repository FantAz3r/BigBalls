using TMPro;
using UnityEngine;
using VContainer;

public class GlobalWalletView : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyText;
    private GlobalWallet _globalWallet;

    [Inject]
    public void Construct(GlobalWallet globalWallet)
    {
        _globalWallet = globalWallet;
        _globalWallet.OnValueChange += View;
        View();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(_globalWallet != null)
            _globalWallet.OnValueChange -= View;
    }

    private void View()
    {
        _moneyText.text = _globalWallet.Coins.ToString();
    }
}
