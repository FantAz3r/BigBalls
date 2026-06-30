using System;
using UnityEngine;

public class GlobalWallet
{
    private ISaveService _saveService;

    public GlobalWallet(ISaveService saveService)
    {
        _saveService = saveService;
    }

    public void TakeCollectedCoin(float coin)
    {
        _saveService.GameProgress.Coins += Convert.ToInt32(coin);

        Debug.Log("Collected Coin: " + _saveService.GameProgress.Coins);
        _saveService.Save();
    }
}