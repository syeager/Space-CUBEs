// Steve Yeager
// 1.15.2014

using Annotations;
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Runs all methods needed at game start.
/// </summary>
public class GameStart : Singleton<GameStart>
{
    #region Public Fields

    public int version;
    public bool resetVersion;

    #endregion

    #region Readonly Fields

    private static readonly List<Action> StartActions = new List<Action> { Version1 };

    #endregion

    #region Const Fields

    private const string Alpha001 = "Test Build|33|33|44|30|2/(4.0, 5.0, 4.0)/(0.0, 0.0, 0.0)/-1/12~30~|16/(6.0, 5.0, 4.0)/(0.0, 0.0, 0.0)/-1/17~|16/(3.0, 5.0, 4.0)/(0.0, 0.0, 180.0)/-1/17~|7/(5.0, 4.0, 3.0)/(0.0, 0.0, 180.0)/-1/12~29~|12/(7.0, 4.0, 3.0)/(90.0, 180.0, 0.0)/-1/14~20~|12/(2.0, 4.0, 3.0)/(90.0, 180.0, 0.0)/-1/14~20~|100/(3.0, 4.0, 5.0)/(0.0, 0.0, 180.0)/0/7~27~|101/(6.0, 4.0, 5.0)/(0.0, 0.0, 180.0)/1/17~23~|103/(5.0, 4.0, 5.0)/(0.0, 0.0, 180.0)/2/14~25~21~|104/(7.0, 6.0, 4.0)/(0.0, 0.0, 90.0)/3/11~20~|111/(2.0, 6.0, 4.0)/(0.0, 0.0, 90.0)/-1/11~|115/(2.0, 6.0, 5.0)/(270.0, 180.0, 0.0)/-1/20~|5/(3.0, 3.0, 3.0)/(0.0, 0.0, 0.0)/-1/23~30~|5/(6.0, 3.0, 3.0)/(0.0, 0.0, 0.0)/-1/23~30~|";
    private const string Rage = "Killer|36|36|62|40|1/(5.0, 5.0, 5.0)/(0.0, 270.0, 270.0)/-1/14~31~|7/(6.0, 4.0, 2.0)/(0.0, 0.0, 90.0)/-1/14~30~|7/(3.0, 4.0, 2.0)/(0.0, 0.0, 90.0)/-1/14~30~|19/(6.0, 4.0, 4.0)/(0.0, 0.0, 0.0)/-1/1~|19/(3.0, 4.0, 5.0)/(0.0, 180.0, 0.0)/-1/1~|13/(5.0, 5.0, 4.0)/(0.0, 90.0, 270.0)/-1/0~28~|13/(4.0, 5.0, 4.0)/(0.0, 90.0, 270.0)/-1/0~28~|102/(7.0, 3.0, 4.0)/(0.0, 0.0, 180.0)/1/17~1~|105/(2.0, 3.0, 4.0)/(0.0, 0.0, 180.0)/2/17~1~|106/(5.0, 3.0, 4.0)/(0.0, 0.0, 90.0)/3/12~|100/(3.0, 6.0, 4.0)/(0.0, 0.0, 90.0)/0/12~29~|100/(6.0, 6.0, 4.0)/(0.0, 0.0, 90.0)/-1/12~29~|111/(5.0, 4.0, 4.0)/(0.0, 90.0, 270.0)/-1/29~|111/(4.0, 4.0, 4.0)/(0.0, 90.0, 270.0)/-1/29~|113/(5.0, 4.0, 3.0)/(0.0, 270.0, 90.0)/-1/29~|113/(5.0, 4.0, 2.0)/(0.0, 270.0, 90.0)/-1/29~|113/(4.0, 4.0, 3.0)/(0.0, 270.0, 90.0)/-1/29~|113/(4.0, 4.0, 2.0)/(0.0, 270.0, 90.0)/-1/29~|";

    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    protected override void Awake()
    {
        base.Awake();
        LoadGame();
        UpdateVersions();
    }

    #endregion


    #region Private Methods

    private static void LoadGame()
    {
        CUBE.LoadAllCUBEInfo();
    }


    private void UpdateVersions()
    {
        int previousVersion = resetVersion ? 0 : PlayerPrefs.GetInt("Build Version");
        while (previousVersion < version)
        {
            Debugger.Log("GameStart: " + previousVersion, gameObject, Debugger.LogTypes.Data, true);
            StartActions[previousVersion].Invoke();
            previousVersion++;
        }
        PlayerPrefs.SetInt("Build Version", version);
    }


    private static void Version1()
    {
        // initial inventory
        int count = CUBE.allCUBES.Length;

        int[] inventory = new int[count];
        for (int i = 0; i < count; i++)
        {
            inventory[i] = 100;
        }

        CUBE.SetInventory(inventory);

        // initial bank
        MoneyManager.SetBalance(10000);

        // default ship build
        ConstructionGrid.SaveBuild("Test Build", Alpha001);
        ConstructionGrid.SaveBuild("Rage", Rage);
    }

    #endregion
}