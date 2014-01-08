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


    #region Public Methods

    public void Collect(int amount)
    {
        money += amount;
    }


    public void Save()
    {
        var balance = Balance();
        PlayerPrefs.SetInt(MONEYPATH, money+balance);
        money = 0;
    }

    #endregion

    #region Static Methods

    public static int Balance()
    {
        return PlayerPrefs.GetInt(MONEYPATH);
    }


    public static bool Transaction(int amount)
    {
        int balance = Balance();
        if (balance < amount)
        {
            return false;
        }
        balance -= amount;
        PlayerPrefs.SetInt(MONEYPATH, balance);
        return true;
    }

    #endregion
}
