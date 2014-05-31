// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.05
// Edited: 2014.05.31

using System.Collections.Generic;

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

// ReSharper disable InconsistentNaming
    public static string Empty
// ReSharper restore InconsistentNaming
    {
        get { return "" + DataSep + "0" + DataSep + "0" + DataSep + "0" + DataSep + "0" + DataSep + "0"; }
    }

    #endregion
}