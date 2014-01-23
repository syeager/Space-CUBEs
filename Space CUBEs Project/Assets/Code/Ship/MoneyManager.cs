// Steve Yeager
// 1.6.2014

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 
/// </summary>
[Serializable]
public class MoneyManager
{
    #region Properties

    public int money { get; private set; }

    #endregion

    #region Const Fields

    private const string MONEYPATH = "Money: ";

    #endregion

    #region Events

    public EventHandler<CashUpdateArgs> CashUpdateEvent;

    #endregion


    #region Public Methods

    public void Collect(int amount)
    {
        money += amount;
        if (CashUpdateEvent != null)
        {
            CashUpdateEvent(this, new CashUpdateArgs(money));
        }
    }


    public void Save()
    {
        var balance = Balance();
        PlayerPrefs.SetInt(MONEYPATH, money+balance);
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
        return PlayerPrefs.GetInt(MONEYPATH);
    }


    /// <summary>
    /// Sets balance.
    /// </summary>
    /// <remarks>Only use in Editor.</remarks>
    /// <param name="balance">Balance to set.</param>
    public static void SetBalance(int balance)
    {
        PlayerPrefs.SetInt(MONEYPATH, balance);
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
        PlayerPrefs.SetInt(MONEYPATH, balance);
        return balance;
    }

    #endregion
}
