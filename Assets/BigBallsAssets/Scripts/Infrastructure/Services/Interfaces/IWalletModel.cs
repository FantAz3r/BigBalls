using System;

public interface IWalletModel
{
    float CurrentValue { get; }

    event Action<IWalletModel> OnValueChanged;
    event Action<float> OnCoinsCollected;
    
    void AddCoins(int value);
    void PutAccumulatedCoins();
}