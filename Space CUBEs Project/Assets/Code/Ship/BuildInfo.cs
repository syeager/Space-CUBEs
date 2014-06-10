// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.08

using System;
using System.Collections.Generic;


[Serializable]
public class BuildInfo
{
    #region Public Fields

    public string name;

    public float health;

    public float shield;

    public float speed;

    public float damage;

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
}