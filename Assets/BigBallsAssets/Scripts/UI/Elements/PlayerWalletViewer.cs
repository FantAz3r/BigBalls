using TMPro;
using UnityEngine;
using VContainer;

public class PlayerWalletViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;

    private IWalletModel _walletModel;
    
    [Inject]
    public void Init(IWalletModel walletModel)
    {
        _walletModel = walletModel;
        _valueText.text = $"{_walletModel.CurrentValue}";
        
        HandlerOnOnValueChanged(_walletModel);

        _walletModel.OnValueChanged += HandlerOnOnValueChanged;
    }

    private void OnDisable()
    {
        _walletModel.OnValueChanged -= HandlerOnOnValueChanged;
    }

    private void HandlerOnOnValueChanged(IWalletModel walletModel)
    {
        _valueText.text = $"{walletModel.CurrentValue}";
    }
}
