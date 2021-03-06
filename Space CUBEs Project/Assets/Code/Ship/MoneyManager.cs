﻿// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.10
// Edited: 2014.06.04

using System;
using LittleByte.Data;
using LittleByte.Extensions;

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

    private const string MoneyFile = "Money";
    private const string BankFolder = @"Bank/";
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
        SaveData.Save(MoneyFile, money + balance, BankFolder);
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
        return SaveData.Load(MoneyFile, BankFolder, BankStart);
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
        SaveData.Save(MoneyFile, balance, BankFolder);
        return balance;
    }

    #endregion
}