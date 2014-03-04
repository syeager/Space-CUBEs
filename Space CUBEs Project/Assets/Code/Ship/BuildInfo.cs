// Steve Yeager
// 12.5.2013

using System.Collections.Generic;
using UnityEngine;

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

    public const string DATASEP = "|";
    public const string PIECESEP = "/";
    public const string COLORSEP = "~";

    #endregion

    #region Properties

    [System.Obsolete()]
    public static string Empty
    {
        get { return "" + DATASEP + "0" + DATASEP + "0" + DATASEP + "0" + DATASEP + "0"; }
    }

    #endregion
}