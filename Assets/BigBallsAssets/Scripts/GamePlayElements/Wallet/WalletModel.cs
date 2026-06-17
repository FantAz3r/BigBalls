using UnityEngine;

public class WalletModel : IWalletModel
{
    public void AddCoins(int value)
    {
        Debug.Log($"Добавлено {value} золота");
    }
}