using System;
using BigBalls.GameplayObjects;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private LootType _type;
    private int _value;
    
    public event Action <Loot> OnLoot;
    
    public void Initialize(LootType type, int value)
    {
        _type = type;
        _value = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            ApplyLoot(player);
            
            OnLoot?.Invoke(this);
        }
    }
    
    private void ApplyLoot(Player player)
    {
        switch (_type)
        {
            case LootType.Coin:
                Debug.Log($"Играк подобрал монеты в количестве {_value} штук");
                // player.AddCoins(_value);
                break;
            case LootType.HealthPotion:
                Debug.Log($"Играк увеличил свое здоровье на {_value} единиц");
                // player.Heal(_value);
                break;
        }
    }
}
