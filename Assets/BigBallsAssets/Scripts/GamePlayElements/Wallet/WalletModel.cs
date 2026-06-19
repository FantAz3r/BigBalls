using System;

public class WalletModel : IWalletModel
{
    public float CurrentValue { get; private set; }
    public event Action<IWalletModel> OnValueChanged;

    public void AddCoins(int value)
    {
        CurrentValue += value;
        OnValueChanged?.Invoke(this);
    }
}