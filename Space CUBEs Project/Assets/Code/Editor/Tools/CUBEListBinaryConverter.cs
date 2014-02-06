// Steve Yeager
// 1.15.2014

using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

using Types = CUBE.Types;
using Subsystems = CUBE.Subsystems;
using Brands = CUBE.Brands;

/// <summary>
/// Converts CUBE List XML to a binary file.
/// </summary>
public class CUBEListBinaryConverter : EditorWindow
{
    #region Readonly Fields

    private static readonly Vector2 SIZE = new Vector2(300f, 50f);
    private static readonly string CUBECSVPATH = Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Data\CUBE List.csv";
    public static readonly string CUBELISTPATHEDITOR = Application.dataPath + "/Resources/CUBE List.bytes";

    #endregion


    #region EditorWindow Overrides

    [MenuItem("Tools/CUBE List Binary Converter")]
    private static void Init()
    {
        CUBEListBinaryConverter window = (CUBEListBinaryConverter)EditorWindow.GetWindow<CUBEListBinaryConverter>(true, "CUBE List Binary Converter");
        window.minSize = window.maxSize = SIZE;
    }


    private void OnGUI()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Convert"))
        {
            ToBinary(CSVToCUBEInfo());
        }
    }

    #endregion

    #region Private Methods

    private CUBEInfo[] CSVToCUBEInfo()
    {
        List<CUBEInfo> info = new List<CUBEInfo>();

        // get info from csv file
        string[] infoStrips = File.ReadAllText(CUBECSVPATH).Split('\n');

        for (int i = 1; i < infoStrips.Length; i++)
        {
            // break line down into data
            string[] strip = infoStrips[i].Split(',');

            info.Add(new CUBEInfo(
                         strip[0],                                              // name
                         int.Parse(strip[1]),                                   // ID
                         (Types)Enum.Parse(typeof(Types), strip[2]),            // type
                         (Subsystems)Enum.Parse(typeof(Subsystems), strip[3]),  // subsystem
                         (Brands)Enum.Parse(typeof(Brands), strip[4]),          // brand
                         int.Parse(strip[5]),                                   // grade
                         int.Parse(strip[6]),                                   // limit
                         float.Parse(strip[7]),                                 // health
                         float.Parse(strip[8]),                                 // shield
                         float.Parse(strip[9]),                                 // speed
                         float.Parse(strip[10]),                                // damage
                         Utility.ParseV3(strip[11], ';'),                       // size
                         int.Parse(strip[12]),                                  // cost
                         int.Parse(strip[13]),                                  // rarity
                         int.Parse(strip[14])                                   // price
                         ));
        }

        return info.ToArray();
    }


    private void ToBinary(CUBEInfo[] info)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(CUBELISTPATHEDITOR, FileMode.Create)))
        {
            foreach (var cube in info)
            {
                writer.Write(cube.name);                    // name
                writer.Write(cube.ID);                      // ID
                writer.Write((int)cube.type);         // type
                writer.Write((int)cube.subsystem);    // subsystem
                writer.Write((int)cube.brand);        // brand
                writer.Write(cube.grade);                   // grade
                writer.Write(cube.limit);                   // limit
                writer.Write(cube.health);                  // health
                writer.Write(cube.shield);                  // shield
                writer.Write(cube.speed);                   // speed
                writer.Write(cube.damage);                  // damage
                writer.Write(cube.size.ToString());         // size
                writer.Write(cube.cost);                    // cost
                writer.Write(cube.rarity);                  // rarity
                writer.Write(cube.price);                   // price
            }
        }

        Debug.Log("File updated successfully.");
    }

    #endregion
}