using System;

public interface IWalletModel
{
    float CurrentValue { get; }
    event Action<IWalletModel> OnValueChanged;
    void AddCoins(int value);
    void PutAccumulatedCoins();
}