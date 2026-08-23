using System;

public class GlobalWallet
{
    private ISaveService _saveService;
    public int Coins { get; private set; }

    public GlobalWallet(ISaveService saveService)
    {
        _saveService = saveService;
        Coins = _saveService.GameProgress.Coins;
    }

    public event Action OnValueChange;

    public bool TrySpendCoins(int cost)
    {
        if (cost <= Coins)
        {
            Coins -= cost;
            OnValueChange?.Invoke();
            return true;
        }

        return false;
    }

    public void AddCoins(float coin)
    {
        _saveService.GameProgress.Coins += Convert.ToInt32(coin);
        _saveService.Save();
    }
}