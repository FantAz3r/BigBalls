using System;

public class GlobalWallet
{
    private ISaveService _saveService;
    public int Coins { get; private set; }

    public GlobalWallet (ISaveService saveService)
    {
        _saveService = saveService;
        Coins = _saveService.GameProgress.Coins;
    }

    public event Action OnValueChange;
    public event Action Rejected;

    public bool CanSpendCoins(int cost)
    {
        return cost <= Coins;
    }

    public bool TrySpendCoins (int cost)
    {
        if (CanSpendCoins(cost))
        {
            Coins -= cost;
            OnValueChange?.Invoke();
            return true;
        }

        Rejected?.Invoke();
        return false;
    }

    public void AddCoins (float coin)
    {
        _saveService.GameProgress.Coins += Convert.ToInt32(coin);
        _saveService.Save();
    }
}