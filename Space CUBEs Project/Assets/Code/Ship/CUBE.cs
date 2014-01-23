// Steve Yeager
// 11.26.2013

using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CUBE : MonoBehaviour
{
    #region Public Fields

    public int ID = -1;
    public Vector3[] pieces = new Vector3[0];

    public enum Types
    {
        Armor = 0,
        Weapon = 1,
        Cockpit = 2,
        Engine = 3,
        Wing = 4,
    }
    public Types type;

    public float health;
    public float shield;
    public float speed;
    public int rarity;
    public int price;

    #endregion

    #region Static Fields

    public static CUBEInfo[] allCUBES { get; private set; }
    public static int[][] gradedCUBEs { get; private set; }

    #endregion

    #region Const Fields

    public const string CUBELIST = "CUBE List";
    private const string INVENTORYPATH = "Inventory";
    private const char CUBESEPARATER = '|';

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
                            reader.ReadInt32(),
                            reader.ReadString(),
                            (Types)reader.ReadInt32(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadInt32(),
                            reader.ReadInt32()
                        ));
            }
        }
        allCUBES = infoList.ToArray();

        if (Application.isPlaying)
        {
            Debugger.Log("CUBE info loaded from binary.", null, true, Debugger.LogTypes.Data);
        }

        // filter into graded
        gradedCUBEs = new int[5][];
        for (int i = 0; i < 5; i++)
        {
            gradedCUBEs[i] = allCUBES.Where(c => c.rarity == i + 1).Select(c => c.id).ToArray();
        }

        return allCUBES;
    }


    public static int[] GetInventory()
    {
        string[] data = PlayerPrefs.GetString(INVENTORYPATH, "").Replace(" ", "").Split(CUBESEPARATER);
        int[] inventory = new int[CUBE.allCUBES.Length];

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
        PlayerPrefs.SetString(INVENTORYPATH, string.Join(CUBESEPARATER.ToString(), inventory.Select(c => c.ToString()).ToArray()));
    }

    #endregion
}