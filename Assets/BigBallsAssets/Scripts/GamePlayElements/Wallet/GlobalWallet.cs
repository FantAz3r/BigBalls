using System;
using UnityEngine;

public class GlobalWallet : IDisposable
{
    private ISaveService _saveService;
    private IWalletModel _walletModel;

    public GlobalWallet(ISaveService saveService, IWalletModel walletModel)
    {
        _saveService = saveService;
        _walletModel = walletModel;
        _walletModel.OnCoinsCollected += TakeCollectedCoin;
    }
    
    public void Dispose()
    {
        _walletModel.OnCoinsCollected -= TakeCollectedCoin;
    }

    private void TakeCollectedCoin(float coin)
    {
        _saveService.GameProgress.Coins += Convert.ToInt32(coin);
        
        Debug.Log("Collected Coin: " + _saveService.GameProgress.Coins);
        _saveService.Save();
    }
}