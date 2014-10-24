// Little Byte Games
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.10.22

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[Serializable]
public class BuildInfo
{
    #region Public Fields

    public string name;

    public ShipStats stats;

    public int trimColor;

    public List<KeyValuePair<int, CUBEGridInfo>> partList;

    #endregion

    #region Static Fields

    public const string DataSep = "|";
    public const string PieceSep = "/";
    public const string ColorSep = "~";

    #endregion

    #region Properties

    public static string Empty
    {
        get { return "" + DataSep + "0" + DataSep + "0" + DataSep + "0" + DataSep + "0" + DataSep + "0"; }
    }

    #endregion

    #region Constructors

    public BuildInfo()
    {
        stats = new ShipStats();
        partList = new List<KeyValuePair<int, CUBEGridInfo>>();
    }


    public BuildInfo(string name, ShipStats stats, int trimColor, ICollection<KeyValuePair<CUBE, CUBEGridInfo>> info)
    {
        this.name = name;
        this.stats = stats;
        this.trimColor = trimColor;

        partList = new List<KeyValuePair<int, CUBEGridInfo>>(info.Count);
        foreach (var entry in info)
        {
            partList.Add(new KeyValuePair<int, CUBEGridInfo>(entry.Key.ID, entry.Value));
        }
    }

    #endregion

    #region Object Overrides

    public override string ToString()
    {
        return name;
    }

    #endregion
}