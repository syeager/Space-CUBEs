// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.10
// Edited: 2014.06.04

using System;
using LittleByte.Data;

/// <summary>
/// Hnadles the players money.
/// </summary>
[Serializable]
public class MoneyManager
{
    #region Properties

    public int money { get; private set; }

    #endregion

    #region Const Fields

    private const string MoneyKey = "Money";
    private const string BankFile = "Bank";
    private const int BankStart = 100000;

    #endregion

    #region Events

    public EventHandler<CashUpdateArgs> CashUpdateEvent;

    #endregion

    #region Public Methods

    /// <summary>
    /// Add money to the local cache.
    /// </summary>
    /// <param name="amount">Amount to add.</param>
    public void Collect(int amount)
    {
        money += amount;
        CashUpdateEvent.Fire(this, new CashUpdateArgs(money));
    }


    /// <summary>
    /// Pushes cached money to the bank in saved data.
    /// </summary>
    public void Save()
    {
        int balance = Balance();
        SaveData.Save(MoneyKey, money + balance, BankFile);
        money = 0;
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Get current balance.
    /// </summary>
    /// <returns>Current balance.</returns>
    public static int Balance()
    {
        return SaveData.Load(MoneyKey, BankFile, BankStart);
    }


    /// <summary>
    /// Add amount to current savings.
    /// </summary>
    /// <param name="amount">Amount to add.</param>
    /// <returns>New balance.</returns>
    public static int Transaction(int amount)
    {
        int balance = Balance();
        balance += amount;
        SaveData.Save(MoneyKey, balance, BankFile);
        return balance;
    }

    #endregion
}