// Steve Yeager
// 1.15.2014

using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

/// <summary>
/// Converts CUBE List XML to a binary file.
/// </summary>
public class CUBEListBinaryConverter : EditorWindow
{
    #region Readonly Fields

    private static readonly Vector2 SIZE = new Vector2(300f, 50f);
    private static readonly string CUBEXMLPATH = Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Data\CUBE List.xml";

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
        if (GUILayout.Button("Convert"))
        {
            ToBinary(ToCUBEInfo());
        }
    }

    #endregion

    #region Private Methods

    private CUBEInfo[] ToCUBEInfo()
    {
        List<CUBEInfo> info = new List<CUBEInfo>();
        int cID = -1;
        string cName = "";
        CUBE.Types cType = CUBE.Types.Armor;
        float cHealth = 0f, cShield = 0f, cSpeed = 0f;
        int cRarity = 0, cPrice = 0;

        using (XmlReader reader = XmlReader.Create(CUBEXMLPATH))
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "CUBE":
                            cID = int.Parse(reader["ID"]);
                            cName = reader["Name"];
                            cType = (CUBE.Types)Enum.Parse(typeof(CUBE.Types), reader["Type"]);
                            break;

                        case "Health":
                            reader.Read();
                            cHealth = float.Parse(reader.Value);
                            break;
                        case "Shield":
                            reader.Read();
                            cShield = float.Parse(reader.Value);
                            break;
                        case "Speed":
                            reader.Read();
                            cSpeed = float.Parse(reader.Value);
                            break;

                        case "Rarity":
                            reader.Read();
                            cRarity = int.Parse(reader.Value);
                            break;
                        case "Price":
                            reader.Read();
                            cPrice = int.Parse(reader.Value);
                            info.Add(new CUBEInfo(cID, cName, cType, cHealth, cShield, cSpeed, cRarity, cPrice));
                            break;
                    }
                }
            }
        }

        return info.ToArray();
    }


    private void ToBinary(CUBEInfo[] info)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(CUBE.CUBEDATPATH, FileMode.Create)))
        {
            foreach (var cube in info)
            {
                writer.Write(cube.id);
                writer.Write(cube.name);
                writer.Write((int)cube.type);
                writer.Write(cube.health);
                writer.Write(cube.shield);
                writer.Write(cube.speed);
                writer.Write(cube.rarity);
                writer.Write(cube.price);
            }
        }

        Debug.Log("File updated successfully.");
    }

    #endregion
}