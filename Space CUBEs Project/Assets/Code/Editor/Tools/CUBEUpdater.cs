// Steve Yeager
// 1.15.2014

using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

using Types = CUBE.Types;
using Subsystems = CUBE.Subsystems;
using Brands = CUBE.Brands;
using Object = UnityEngine.Object;

/// <summary>
/// 
/// </summary>
public class CUBEUpdater : EditorWindow
{
    #region Readonly Fields

    private static readonly Vector2 SIZE = new Vector2(300f, 50f);
    private static readonly string CUBECSVPATH = Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Data\CUBE List.csv";
    private static readonly string CUBELISTPATHEDITOR = Application.dataPath + "/Resources/CUBE List.bytes";
    private const string PREFABPATH = "Assets/Ship/CUBEs/Prefabs/";
    private const string GAMERESOURCESPATH = "Assets/Global/";

    #endregion


    #region EditorWindow Overrides

    [MenuItem("Tools/CUBE Updater")]
    private static void Init()
    {
        CUBEUpdater window = (CUBEUpdater)EditorWindow.GetWindow<CUBEUpdater>(true, "CUBE Updater");
        window.minSize = window.maxSize = SIZE;
    }


    private void OnGUI()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Update"))
        {
            Update();
        }
    }

    #endregion

    #region Static Methods

    private static CUBEInfo[] CSVToCUBEInfo()
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


    private static void ToBinary(CUBEInfo[] info)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(CUBELISTPATHEDITOR, FileMode.Create)))
        {
            foreach (var cube in info)
            {
                writer.Write(cube.name);            // name
                writer.Write(cube.ID);              // ID
                writer.Write((int)cube.type);       // type
                writer.Write((int)cube.subsystem);  // subsystem
                writer.Write((int)cube.brand);      // brand
                writer.Write(cube.grade);           // grade
                writer.Write(cube.limit);           // limit
                writer.Write(cube.health);          // health
                writer.Write(cube.shield);          // shield
                writer.Write(cube.speed);           // speed
                writer.Write(cube.damage);          // damage
                writer.Write(cube.size.ToString()); // size
                writer.Write(cube.cost);            // cost
                writer.Write(cube.rarity);          // rarity
                writer.Write(cube.price);           // price
            }
        }
    }


    private static void UpdatePrefabs()
    {
        // get all CUBE prefabs
        var prefabs = Utility.LoadObjects<CUBE>(PREFABPATH).ToArray();

        CUBEInfo[] info = CUBE.LoadAllCUBEInfo();

        // update prefab
        SerializedObject serializedPrefab;
        for (int i = 0; i < prefabs.Length; i++)
        {
            // set ID
            serializedPrefab = new SerializedObject(prefabs[i]);
            Debug.Log(prefabs[i].name);
            serializedPrefab.FindProperty("ID").intValue = info.First(c => c.name == prefabs[i].name).ID;
            serializedPrefab.ApplyModifiedProperties();
        }

        // get GameResources
        GameResources gameResources = Utility.LoadObject<GameMaster>(GAMERESOURCESPATH).GetComponentsInChildren<GameResources>(true)[0];
        SerializedObject resources = new SerializedObject(gameResources);
        SerializedProperty resourcesPrefabs = resources.FindProperty("CUBE_Prefabs");

        // update GameResources array size
        resources.Update();
        while (resourcesPrefabs.arraySize < info.Length)
        {
            resourcesPrefabs.InsertArrayElementAtIndex(resourcesPrefabs.arraySize);
        }

        // set all prefabs
        for (int i = 0; i < info.Length; i++)
        {
            CUBE[] prefab = prefabs.Where(p => p.ID == i).ToArray();
            resourcesPrefabs.GetArrayElementAtIndex(i).objectReferenceValue = prefab.Length > 0 ? prefab[0].gameObject : null;
        }
        resources.ApplyModifiedProperties();
    }


    public static void Update()
    {
        DateTime startTime = DateTime.Now;
        ToBinary(CSVToCUBEInfo());
        UpdatePrefabs();
        Debug.Log("CUBE list updated successfully. " + (DateTime.Now - startTime).TotalSeconds);
    }

    #endregion
}