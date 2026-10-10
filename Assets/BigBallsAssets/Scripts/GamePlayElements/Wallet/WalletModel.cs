using System;
using BigBalls.Services;
using UnityEngine;

public class WalletModel : IWalletModel
{
    private GlobalWallet _globalWallet;
    private DamageTextFactory _textFactory;
    private IPlayerProvider _playerProvider;
    private string _goldPrefix = "$";

    public WalletModel (GlobalWallet globalWallet, DamageTextFactory damageTextFactory, IPlayerProvider playerProvider)
    {
        _globalWallet = globalWallet;
        _playerProvider = playerProvider;
        _textFactory = damageTextFactory;
    }

    public event Action<IWalletModel> OnValueChanged;

    public float CurrentValue { get; private set; }

    public void AddCoins (int value)
    {
        CurrentValue += value;
        _textFactory.Create(_playerProvider.Player.Transform.position, value, _goldPrefix, Color.yellow);
        OnValueChanged?.Invoke(this);
    }

    public bool TrySpendCoins (int value)
    {
        if (CurrentValue < value)
        {
            return false;
        }
        else
        {
            CurrentValue -= value;
            OnValueChanged?.Invoke(this);
            return true;
        }
    }

    public void PutAccumulatedCoins ()
    {
        _globalWallet.AddCoins(CurrentValue);
    }
}