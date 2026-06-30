using System;

public class WalletModel : IWalletModel
{
    private GlobalWallet _globalWallet;
    
    public WalletModel(GlobalWallet globalWallet)
    {
        _globalWallet = globalWallet;
    }
    
    public event Action<IWalletModel> OnValueChanged;
    
    public float CurrentValue { get; private set; }
    
    public void AddCoins(int value)
    {
        CurrentValue += value;
        OnValueChanged?.Invoke(this);
    }

    public void PutAccumulatedCoins()
    {
        _globalWallet.TakeCollectedCoin(CurrentValue);
    }
}