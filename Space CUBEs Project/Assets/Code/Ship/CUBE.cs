// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.11.26
// Edited: 2014.06.06

using System.Collections.Generic;
using System.IO;
using System.Linq;
using LittleByte.Data;
using UnityEngine;

public class CUBE : MonoBehaviour
{
    #region Public Fields

    public int ID = -1;

    public enum Types
    {
        System,
        Hull,
        Weapon,
        Augmentation,
    }

    public enum Subsystems
    {
        None,
        Cockpit,
        Engine,
        PowerCore,
        Wing,
    }

    public enum Brands
    {
        None,
        Omni,
        Vita,
        Aegis,
        Drift,
        Titan,
    }

    #endregion

    #region Static Fields

    public static CUBEInfo[] allCUBES { get; private set; }
    public static int[][] gradedCUBEs { get; private set; }
    public static Color[] colors { get; private set; }

    #endregion

    #region Const Fields

    public const string CUBELIST = "CUBE List";
    public const string COLORLIST = "Color List";
    private const string InventoryFile = "Inventory";
    private const string ItemsFolder = @"Items/";
    private const char CUBESEP = '|';

    public const string HEALTHICON = "♥";
    public const string SHIELDICON = "Θ";
    public const string SPEEDICON = "►";
    public const string DAMAGEICON = "•";

    #endregion

    #region Static Methods

    public static CUBEInfo[] LoadAllCUBEInfo()
    {
        // load all CUBEs
        var binaryFile = (TextAsset)Resources.Load(CUBELIST);
        Stream binaryStream = new MemoryStream(binaryFile.bytes);
        var infoList = new List<CUBEInfo>();
        using (var reader = new BinaryReader(binaryStream))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                infoList.Add(new CUBEInfo
                    (
                    reader.ReadString(), // name
                    reader.ReadInt32(), // ID
                    (Types)reader.ReadInt32(), // type
                    (Subsystems)reader.ReadInt32(), // subsystem
                    (Brands)reader.ReadInt32(), // brand
                    reader.ReadInt32(), // grade
                    reader.ReadSingle(), // health
                    reader.ReadSingle(), // shield
                    reader.ReadSingle(), // speed
                    reader.ReadSingle(), // damage
                    Utility.ParseV3(reader.ReadString()), // size
                    reader.ReadInt32(), // cost
                    reader.ReadInt32(), // rarity
                    reader.ReadInt32() // price
                    ));
            }
        }
        allCUBES = infoList.ToArray();

        if (Application.isPlaying)
        {
            Debugger.Log("CUBE info loaded from binary.", null, Debugger.LogTypes.Data);
        }

        // filter into graded
        gradedCUBEs = new int[5][];
        for (int i = 0; i < 5; i++)
        {
            gradedCUBEs[i] = allCUBES.Where(c => c.rarity == i + 1).Select(c => c.ID).ToArray();
        }

        return allCUBES;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static int[] GetInventory()
    {
        return SaveData.Load(InventoryFile, ItemsFolder, new int[allCUBES.Length]);
    }


    public static void SetInventory(int[] inventory)
    {
        SaveData.Save(InventoryFile, inventory, ItemsFolder);
    }


    public static Color[] LoadColors()
    {
        var binaryFile = (TextAsset)Resources.Load(COLORLIST);
        Stream binaryStream = new MemoryStream(binaryFile.bytes);
        var colors = new List<Color>();
        using (var reader = new BinaryReader(binaryStream))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                colors.Add(Utility.ParseColor(reader.ReadString()));
            }
        }

        CUBE.colors = colors.ToArray();
        return CUBE.colors;
    }


    public static CUBEInfo GetInfo(string cubeName)
    {
#if UNITY_EDITOR
        if (allCUBES == null)
        {
            LoadAllCUBEInfo();
        }
#endif

// ReSharper disable AssignNullToNotNullAttribute
        return allCUBES.First(c => c.name == cubeName);
// ReSharper restore AssignNullToNotNullAttribute
    }

    #endregion
}