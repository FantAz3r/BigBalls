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

    private void OnDestroy()
    {
        _globalWallet.OnValueChange -= View;
    }

    private void View()
    {
        _moneyText.text = _globalWallet.Coins.ToString();
    }
}
