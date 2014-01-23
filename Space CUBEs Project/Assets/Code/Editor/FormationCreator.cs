// Steve Yeager
// 1.12.2014

using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FormationCreator : EditorWindow
{
    #region Private Fields

    private string formationName;
    private bool confirmOverwrite;
    private List<GameObject> formations = new List<GameObject>();
    private GameObject oldFormation;
    private GameObject prefab;

    #endregion

    #region Static Fields

    private static readonly Vector2 SIZE = new Vector2(300f, 50f);

    #endregion

    #region Const Fields

    private const string FORMATIONPATH = "Assets/Formations/";
    
    #endregion
    

    #region EditorWindow Overrides

    [MenuItem("Tools/Formation Creator")]
    private static void Init()
    {
        FormationCreator window = (FormationCreator)EditorWindow.GetWindow(typeof(FormationCreator), true, "Formation Creator");
    }


    private void OnEnable()
    {
        LoadFormations();
    }


    private void OnDisable()
    {
        DestroyImmediate(prefab);
    }


    private void OnGUI()
    {
        // load or create
        if (prefab == null)
        {
            if (GUILayout.Button("New Formation"))
            {
                prefab = new GameObject("___Formation", typeof(Formation));
                var placeholder = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Sphere);
                placeholder.transform.parent = prefab.transform;
                placeholder.transform.localPosition = Vector3.zero;
            }
            foreach (var p in formations)
            {
                if (GUILayout.Button(p.name))
                {
                    prefab = new GameObject("___Formation", typeof(Formation));
                    formationName = p.name;
                    foreach (var position in p.GetComponent<Formation>().positions)
                    {
                        var placeholder = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        placeholder.transform.parent = prefab.transform;
                        placeholder.transform.localPosition = position;
                    }
                }
            }
            return;
        }

        // pattern name
        formationName = EditorGUILayout.TextField(formationName);

        GUILayout.FlexibleSpace();

        // overwrite
        if (confirmOverwrite)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Cancel"))
                {
                    confirmOverwrite = false;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Overwrite " + formationName))
                {
                    Save(true);
                    confirmOverwrite = false;
                }
            }
        }
        else
        {
            // save
            if (GUILayout.Button("Save"))
            {
                // test for alreay there
                oldFormation = formations.FirstOrDefault(p => p.name == formationName);
                if (oldFormation == null)
                {
                    Save(false);
                }
                else
                {
                    confirmOverwrite = true;
                }
            }
        }
    }

    #endregion

    #region Private Methods

    private void Save(bool overwrite)
    {
        // delete old
        if (overwrite)
        {
            Debug.Log("Deleting: " + FORMATIONPATH + oldFormation.name);
            AssetDatabase.DeleteAsset(FORMATIONPATH + oldFormation.name + ".prefab");
        }

        GameObject savedPrefab = new GameObject(formationName, typeof(Formation));
        var formationComp = savedPrefab.GetComponent<Formation>();
        formationComp.positions = new Vector3[prefab.transform.childCount];
        for (int i = 0; i < formationComp.positions.Length; i++)
        {
            savedPrefab.GetComponent<Formation>().positions[i] = prefab.transform.GetChild(i).localPosition;
        }
        PrefabUtility.CreatePrefab(FORMATIONPATH + "Formation " + formationName + ".prefab", savedPrefab);
        DestroyImmediate(savedPrefab);

        LoadFormations();
    }


    private void LoadFormations()
    {
        string[] formationFiles = Directory.GetFiles(FORMATIONPATH);
        formations.Clear();

        for (int i = 0; i < formationFiles.Length; i++)
        {
            Object formation = AssetDatabase.LoadAssetAtPath(formationFiles[i], typeof(GameObject));
            if (formation != null && PrefabUtility.GetPrefabType(formation) == PrefabType.Prefab)
            {
                formations.Add(formation as GameObject);
            }
        }
    }

    #endregion
}