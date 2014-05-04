// Steve Yeager
// 11.26.2013

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    private const string INVENTORYPATH = "Inventory";
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
        TextAsset binaryFile = (TextAsset)Resources.Load(CUBELIST);
        Stream binaryStream = new MemoryStream(binaryFile.bytes);
        List<CUBEInfo> infoList = new List<CUBEInfo>();
        using (BinaryReader reader = new BinaryReader(binaryStream))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                infoList.Add(new CUBEInfo
                        (
                            reader.ReadString(),                        // name
                            reader.ReadInt32(),                         // ID
                            (Types)reader.ReadInt32(),                  // type
                            (Subsystems)reader.ReadInt32(),             // subsystem
                            (Brands)reader.ReadInt32(),                 // brand
                            reader.ReadInt32(),                         // grade
                            reader.ReadSingle(),                        // health
                            reader.ReadSingle(),                        // shield
                            reader.ReadSingle(),                        // speed
                            reader.ReadSingle(),                        // damage
                            Utility.ParseV3(reader.ReadString()),       // size
                            reader.ReadInt32(),                         // cost
                            reader.ReadInt32(),                         // rarity
                            reader.ReadInt32()                          // price
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


    public static int[] GetInventory()
    {
        string[] data = PlayerPrefs.GetString(INVENTORYPATH, "").Replace(" ", "").Split(CUBESEP);
        int[] inventory = new int[allCUBES.Length];

        if (data.Length != 1)
        {
            for (int i = 0; i < data.Length; i++)
            {
                inventory[i] = int.Parse(data[i]);
            }
        }

        return inventory;
    }


    public static void SetInventory(int[] inventory)
    {
        PlayerPrefs.SetString(INVENTORYPATH, string.Join(CUBESEP.ToString(), inventory.Select(c => c.ToString()).ToArray()));
    }


    public static Color[] LoadColors()
    {
        TextAsset binaryFile = (TextAsset)Resources.Load(COLORLIST);
        Stream binaryStream = new MemoryStream(binaryFile.bytes);
        List<Color> colors = new List<Color>();
        using (BinaryReader reader = new BinaryReader(binaryStream))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                colors.Add(Utility.ParseColor(reader.ReadString()));
            }
        }

        CUBE.colors = colors.ToArray();
        return CUBE.colors;
    }

    #endregion
}