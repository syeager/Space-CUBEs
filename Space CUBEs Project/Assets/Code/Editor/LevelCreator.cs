// Steve Yeager
// 1.26.2014

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

//
public class LevelCreator : EditorWindow
{
    #region Static Fields

    private static FormationLevelManager levelManager;
    private static Type[] pathTypes;
    private static string[] pathNames;

    #endregion

    #region GUI Fields

    float W;
    float H;
    /// <summary>Total height of the formation sections.</summary>
    float formationTotalHeight;
    /// <summary>Height of top bar.</summary>
    private float infoHeight = 50f;
    private float formationHeight = 50f;
    private float formationWidth = 250f;
    private Vector2 formationScroll;
    private float enemyHeight = 20f;

    #endregion

    #region Editor Fields

    private List<bool> formationToggles;
    SerializedObject sLevelManager;
    SerializedProperty sFormationGroups;
    private List<bool[]> enemyToggles;
    private static Formation[] formationPrefabs;
    private const string FORMATIONPATH = "/Formations";
    private static string[] formationNames;

    #endregion


    #region EditorWindow Overrides

    [MenuItem("Tools/Level Creator", true)]
    public static bool Validate()
    {
        levelManager = FindObjectOfType<FormationLevelManager>();
        return levelManager != null;
    }


    [MenuItem("Tools/Level Creator")]
    public static void Init()
    {
        LevelCreator window = (LevelCreator)GetWindow<LevelCreator>("Level Creator");

        window.sLevelManager = new SerializedObject(levelManager);
        window.sFormationGroups = window.sLevelManager.FindProperty("formationGroups");
        
        // formation toggles
        window.formationToggles = new List<bool>(levelManager.formationGroups.Length);
        for (int i = 0; i < levelManager.formationGroups.Length; i++)
        {
            window.formationToggles.Add(false);
        }

        // enemy toggles
        window.enemyToggles = new List<bool[]>(window.formationToggles.Count);
        for (int i = 0; i < window.formationToggles.Count; i++)
        {
            window.enemyToggles.Add(new bool[levelManager.formationGroups[i].enemies.Length]);
        }

        // path types
        pathTypes = Assembly.GetAssembly(typeof(Path)).GetTypes().Where(t => t.Namespace == "Paths").ToArray();
        pathNames = pathTypes.Select(p => p.Name).ToArray();

        // formation prefabs
        formationPrefabs = Formation.AllFormations();
        formationNames = formationPrefabs.Select(f => f.name).ToArray();
    }


    private void OnDisable()
    {
        EditorUtility.UnloadUnusedAssets();
    }


    private void OnGUI()
    {
        //return;
        W = Screen.width;
        H = Screen.height;

        if (InfoBar()) return;
        Formations();
    }

    #endregion

    #region Private Methods

    private bool InfoBar()
    {
        GUI.BeginGroup(new Rect(0f, 0f, W, infoHeight), "", "box");
        {
            // add formation
            if (GUI.Button(new Rect(10f, infoHeight / 2f - 20f, 80f, 40f), "Add"))
            {
                AddFormation();
                return true;
            }

            // open formations
            if (GUI.Button(new Rect(100f, 5f, 120f, 20f), "Open Formations", EditorStyles.miniButtonLeft))
            {
                for (int i = 0; i < formationToggles.Count; i++)
                {
                    formationToggles[i] = true;
                }
            }

            // open enemies
            if (GUI.Button(new Rect(100f, 25f, 120f, 20f), "Open Enemies", EditorStyles.miniButtonLeft))
            {
                for (int i = 0; i < enemyToggles.Count; i++)
                {
                    for (int j = 0; j < enemyToggles[i].Length; j++)
                    {
                        enemyToggles[i][j] = true;
                    }
                }
            }

            // close formations
            if (GUI.Button(new Rect(220f, 5f, 120f, 20f), "Close Formations", EditorStyles.miniButtonRight))
            {
                for (int i = 0; i < formationToggles.Count; i++)
                {
                    formationToggles[i] = false;
                }
            }

            // close enemies
            if (GUI.Button(new Rect(220f, 25f, 120f, 20f), "Close Enemies", EditorStyles.miniButtonRight))
            {
                for (int i = 0; i < enemyToggles.Count; i++)
                {
                    for (int j = 0; j < enemyToggles[i].Length; j++)
                    {
                        enemyToggles[i][j] = false;
                    }
                }
            }
        }
        GUI.EndGroup();

        return false;
    }


    private void Formations()
    {
        formationScroll = GUI.BeginScrollView(new Rect(0f, infoHeight, W, H - infoHeight), formationScroll, new Rect(0f, 0f, W, formationTotalHeight)); // bottom cutting off
        {
            formationTotalHeight = 0f;
            for (int i = 0; i < levelManager.formationGroups.Length; i++)
            {
                formationTotalHeight += DrawFormation(i);
            }
        }
        GUI.EndScrollView();
    }


    private float DrawFormation(int formationIndex)
    {
        sLevelManager.Update();
        FormationGroup formationSeg = levelManager.formationGroups[formationIndex];
        float height = formationHeight;

        // controls
        GUI.BeginGroup(new Rect(0f, formationTotalHeight, formationWidth, formationHeight), "");
        {
            GUI.Label(new Rect(0, 0, 20f, formationHeight / 2f), (formationIndex + 1).ToString());
            int prefabIndex = Array.IndexOf(formationPrefabs, formationSeg.formation);
            EditorGUI.BeginChangeCheck();
            {
                prefabIndex = EditorGUI.Popup(new Rect(22f, 0, formationWidth - 22f, formationHeight / 2f), prefabIndex, formationNames);
            }
            if (EditorGUI.EndChangeCheck())
            {
                UpdateFormation(formationIndex, prefabIndex);
            }

            // toggle view
            if (GUI.Button(new Rect(10f, formationHeight / 2f, 46f, 20f), formationToggles[formationIndex] ? "|" : "O", EditorStyles.miniButtonLeft))
            {
                formationToggles[formationIndex] = !formationToggles[formationIndex];
            }
            // move down
            if (GUI.Button(new Rect(56f, formationHeight / 2f, 46f, 20f), "↓", EditorStyles.miniButtonMid))
            {

            }
            // move up
            if (GUI.Button(new Rect(102f, formationHeight / 2f, 46f, 20f), "↑", EditorStyles.miniButtonMid))
            {

            }
            // duplicate
            if (GUI.Button(new Rect(148f, formationHeight / 2f, 46f, 20f), "+", EditorStyles.miniButtonMid))
            {

            }
            // delete
            if (GUI.Button(new Rect(194f, formationHeight / 2f, 46f, 20f), "-", EditorStyles.miniButtonRight))
            {
                DeleteFormation(formationIndex);
                return height;
            }
        }
        GUI.EndGroup();

        // data
        SerializedProperty sFormSeg = sFormationGroups.GetArrayElementAtIndex(formationIndex);
        GUI.BeginGroup(new Rect(formationWidth, formationTotalHeight, W-formationWidth, formationHeight), "");
        {
            // needs clearing
            EditorGUI.LabelField(new Rect(10f, formationHeight / 2f-10f, 100f, 20f), "Needs Clearing");
            SerializedProperty property = sFormSeg.FindPropertyRelative("needsClearing");
            EditorGUI.PropertyField(new Rect(110f, formationHeight/2f-8f, 16f, 16f), property, new GUIContent(""));

            // spawn time
            EditorGUI.LabelField(new Rect(150f, formationHeight / 2f - 10f, 80f, 20f), "Spawn Time");
            property = sFormSeg.FindPropertyRelative("spawnTime");
            EditorGUI.PropertyField(new Rect(230f, formationHeight / 2f - 10f, 30f, 20f), property, new GUIContent(""));
        }
        GUI.EndGroup();

        sLevelManager.ApplyModifiedProperties();

        if (formationToggles[formationIndex])
        {
            for (int j = 0; j < formationSeg.formation.positions.Length; j++)
            {
                height += Enemy(formationIndex, j, formationTotalHeight + height, sFormSeg);
            }
        }

        EditorGUI.DrawRect(new Rect(formationWidth, formationTotalHeight, 1f, height), Color.grey);
        EditorGUI.DrawRect(new Rect(0f, formationTotalHeight+height, W, 1f), Color.grey);
        return height;
    }


    private float Enemy(int formationIndex, int enemyIndex, float y, SerializedProperty sformationGroup)
    {
        float height = enemyHeight;

        // number
        GUI.Label(new Rect(0f, y, 10f, enemyHeight), (enemyIndex + 1).ToString());
        // toggle
        enemyToggles[formationIndex][enemyIndex] = GUI.Toggle(new Rect(12f, y, 16f, 16f), enemyToggles[formationIndex][enemyIndex], GUIContent.none);
        // enemy
        EditorGUI.PropertyField(new Rect(26f, y, 100f, 20f), sformationGroup.FindPropertyRelative("enemies").GetArrayElementAtIndex(enemyIndex), GUIContent.none);
        // path
        SerializedProperty path = sformationGroup.FindPropertyRelative("paths").GetArrayElementAtIndex(enemyIndex);
        Type pathType = levelManager.formationGroups[formationIndex].paths[enemyIndex].GetType();
        int pathIndex = levelManager.formationGroups[formationIndex].paths[enemyIndex] == null ? -1 : Array.IndexOf(pathTypes, pathType);
        EditorGUI.BeginChangeCheck();
        {
            pathIndex = EditorGUI.Popup(new Rect(126f, y, 100f, 20f), pathIndex, pathNames);
        }
        if (EditorGUI.EndChangeCheck())
        {
            path.objectReferenceValue = ScriptableObject.CreateInstance(pathTypes[pathIndex]);
            sLevelManager.ApplyModifiedProperties();
        }

        // params
        if (enemyToggles[formationIndex][enemyIndex])
        {
            // get params through reflection
            FieldInfo[] fieldInfos = pathType.GetFields();
            SerializedObject pathSO = new SerializedObject(levelManager.formationGroups[formationIndex].paths[enemyIndex]);
            // display params
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (i % 2 == 0)
                {
                    EditorGUI.DrawRect(new Rect(formationWidth, y + height, W - formationWidth, enemyHeight), Color.gray);
                }
                EditorGUI.PropertyField(new Rect(formationWidth + 10f, y + height, W - formationWidth - 20f, enemyHeight), pathSO.FindProperty(fieldInfos[i].Name));
                pathSO.ApplyModifiedProperties();
                height += enemyHeight;
            }
        }

        return height;
    }


    private void AddFormation(int formationIndex = -1)
    {
        sLevelManager.Update();

        // create at end
        if (formationIndex == -1)
        {
            formationIndex = levelManager.formationGroups.Length;
        }

        sFormationGroups.InsertArrayElementAtIndex(formationIndex);
        formationToggles.Insert(formationIndex, false);
        enemyToggles.Insert(formationIndex, null);
        // enemy toggles
        sLevelManager.ApplyModifiedProperties();

        UpdateFormation(formationIndex, 0);
    }


    private void DeleteFormation(int formationIndex)
    {
        sLevelManager.Update();

        // formation
        sFormationGroups.DeleteArrayElementAtIndex(formationIndex);
        // formation toggle
        formationToggles.RemoveAt(formationIndex);
        //enemy toggles
        enemyToggles.RemoveAt(formationIndex);

        sLevelManager.ApplyModifiedProperties();
    }


    private void UpdateFormation(int formationIndex, int prefabIndex)
    {
        SerializedProperty sFormationGroup = sFormationGroups.GetArrayElementAtIndex(formationIndex);

        int enemyCount  = formationPrefabs[prefabIndex].positions.Length;

        // formation
        sFormationGroup.FindPropertyRelative("formation").objectReferenceValue = formationPrefabs[prefabIndex];
        // enemies
        SerializedProperty enemies = sFormationGroup.FindPropertyRelative("enemies");
        enemies.ClearArray();
        for (int i = 0; i < enemyCount; i++)
        {
            enemies.InsertArrayElementAtIndex(0);
        }
        // enemy toggles
        enemyToggles[formationIndex] = new bool[enemyCount];
        // paths
        SerializedProperty paths = sFormationGroup.FindPropertyRelative("paths");
        paths.ClearArray();
        for (int i = 0; i < enemyCount; i++)
        {
            paths.InsertArrayElementAtIndex(i);
            // set to StraightPath
            paths.GetArrayElementAtIndex(i).objectReferenceValue = ScriptableObject.CreateInstance(pathTypes[1]);
        }

        sLevelManager.ApplyModifiedProperties();
        EditorUtility.UnloadUnusedAssets();
    }

    #endregion
}