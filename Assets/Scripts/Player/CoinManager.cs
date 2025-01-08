using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public event Action<int> OnCoinsChanged;

    [SerializeField] private int coins = 0;

    private void Start()
    {
        OnCoinsChanged.Invoke(coins);
    }

    public void AddCoins(int coins)
    {
        this.coins += coins;
        OnCoinsChanged.Invoke(this.coins);
    }

    public void SpendCoins(int coins)
    {
        this.coins -= coins;
        OnCoinsChanged.Invoke(this.coins);
    }

    public int GetCoins() { return coins; }
}
