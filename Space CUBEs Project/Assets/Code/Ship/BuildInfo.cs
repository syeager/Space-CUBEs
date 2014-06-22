// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.21

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 
/// </summary>
[Serializable]
public class BuildInfo
{
    #region Public Fields

    public string name;

    public ShipStats stats;

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


    public BuildInfo(string name, ShipStats stats, ICollection<KeyValuePair<CUBE, CUBEGridInfo>> info)
    {
        this.name = name;
        this.stats = stats;

        partList = new List<KeyValuePair<int, CUBEGridInfo>>(info.Count);
        foreach (var entry in info)
        {
            partList.Add(new KeyValuePair<int, CUBEGridInfo>(entry.Key.ID, entry.Value));
        }
    }


    public BuildInfo(string name, float health, float shield, float speed, float damage, ICollection<KeyValuePair<CUBE, CUBEGridInfo>> info)
    {
        this.name = name;
        stats = new ShipStats(health, shield, speed, damage);

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
        StringBuilder builder = new StringBuilder(name);
        builder.Append(DataSep);
        builder.Append(stats.health);
        builder.Append(DataSep);
        builder.Append(stats.shield);
        builder.Append(DataSep);
        builder.Append(stats.speed);
        builder.Append(DataSep);
        builder.Append(stats.damage);
        builder.Append(DataSep);

        // todo: covert to string
        foreach (var piece in partList)
        {
            builder.Append(piece.Key);
        }


        return builder.ToString();
    }

    #endregion

    #region Operator Overrides

    public static implicit operator BuildInfo(string str)
    {
        BuildInfo buildInfo = new BuildInfo();
        string[] data = str.Split(DataSep[0]);
        Debugger.LogList(data);
        buildInfo.name = data[0];
        buildInfo.stats.health = float.Parse(data[1]);
        buildInfo.stats.shield = float.Parse(data[2]);
        buildInfo.stats.speed = float.Parse(data[3]);
        buildInfo.stats.damage = float.Parse(data[4]);

        for (int i = 5; i < data.Length; i++)
        {
            string[] info = data[i].Split(PieceSep[0]);
            int[] colors = info[5].Substring(0, info[5].Length - 1).Split(ColorSep[0]).Select(s => int.Parse(s)).ToArray();
            buildInfo.partList.Add(new KeyValuePair<int, CUBEGridInfo>(int.Parse(info[0]),
                new CUBEGridInfo(
                    Utility.ParseV3(info[1]),
                    Utility.ParseV3(info[2]),
                    int.Parse(info[3]),
                    int.Parse(info[4]),
                    colors)));
        }

        return buildInfo;
    }

    #endregion
}