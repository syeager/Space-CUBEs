// Steve Yeager
// 1.12.2014

using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PatternCreator : EditorWindow
{
    #region Private Fields

    private string patternName;
    private bool confirmOverwrite;
    private List<GameObject> patterns = new List<GameObject>();
    private GameObject oldPattern;
    private GameObject prefab;

    #endregion

    #region Static Fields

    private static readonly Vector2 SIZE = new Vector2(300f, 50f);

    #endregion

    #region Const Fields

    private const string PATTERNPATH = "Assets/Patterns/";
    
    #endregion
    

    #region EditorWindow Overrides

    [MenuItem("Tools/Pattern Creator")]
    private static void Init()
    {
        PatternCreator window = (PatternCreator)EditorWindow.GetWindow(typeof(PatternCreator), true, "Pattern Creator");
        //window.minSize = SIZE;
        //window.maxSize = SIZE;
    }


    private void OnEnable()
    {
        LoadPatterns();
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
            if (GUILayout.Button("New Pattern"))
            {
                prefab = new GameObject("___Pattern", typeof(Pattern));
                var placeholder = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Sphere);
                placeholder.transform.parent = prefab.transform;
                placeholder.transform.localPosition = Vector3.zero;
            }
            foreach (var p in patterns)
            {
                if (GUILayout.Button(p.name))
                {
                    prefab = new GameObject("___Pattern", typeof(Pattern));
                    patternName = p.name;
                    foreach (var position in p.GetComponent<Pattern>().positions)
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
        patternName = EditorGUILayout.TextField(patternName);

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
                if (GUILayout.Button("Overwrite " + patternName))
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
                oldPattern = patterns.FirstOrDefault(p => p.name == patternName);
                if (oldPattern == null)
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
            Debug.Log("Deleting: " + PATTERNPATH + oldPattern.name);
            AssetDatabase.DeleteAsset(PATTERNPATH + oldPattern.name + ".prefab");
        }

        GameObject savedPrefab = new GameObject(patternName, typeof(Pattern));
        var patternComp = savedPrefab.GetComponent<Pattern>();
        patternComp.positions = new Vector3[prefab.transform.childCount];
        for (int i = 0; i < patternComp.positions.Length; i++)
        {
            savedPrefab.GetComponent<Pattern>().positions[i] = prefab.transform.GetChild(i).localPosition;
        }
        PrefabUtility.CreatePrefab(PATTERNPATH + patternName + " Pattern.prefab", savedPrefab);
        DestroyImmediate(savedPrefab);

        LoadPatterns();
    }


    private void LoadPatterns()
    {
        string[] patternFiles = Directory.GetFiles(PATTERNPATH);
        patterns.Clear();

        for (int i = 0; i < patternFiles.Length; i++)
        {
            Object pattern = AssetDatabase.LoadAssetAtPath(patternFiles[i], typeof(GameObject));
            if (pattern != null && PrefabUtility.GetPrefabType(pattern) == PrefabType.Prefab)
            {
                patterns.Add(pattern as GameObject);
            }
        }
    }

    #endregion
}