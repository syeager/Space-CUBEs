// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.12
// Edited: 2014.05.22

using System.Linq;
using Annotations;
using UnityEditor;
using UnityEngine;

public class FormationCreator : EditorWindow
{
    #region Private Fields

    private string formationName;
    private bool confirmOverwrite;
    private Formation[] formations = new Formation[0];
    private Formation oldFormation;
    private GameObject prefab;

    #endregion

    #region Const Fields

    private const string FormationPrefix = "Formation ";

    #endregion

    #region EditorWindow Overrides

    [UsedImplicitly]
    [MenuItem("CUBEs/Formation Creator %#l", false, 50)]
    private static void Init()
    {
        GetWindow(typeof(FormationCreator), true, "Formation Creator");
    }

    
    [UsedImplicitly]
    private void OnEnable()
    {
        formations = Formation.AllFormations();
    }

    [UsedImplicitly]

    private void OnDisable()
    {
        DestroyImmediate(prefab);
    }


    [UsedImplicitly]
    private void OnGUI()
    {
        // load or create
        if (prefab == null)
        {
            // create
            if (GUILayout.Button("New Formation"))
            {
                prefab = new GameObject("___Formation", typeof(Formation));
                var placeholder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                placeholder.transform.parent = prefab.transform;
                placeholder.transform.localPosition = Vector3.zero;
            }
            // load
            foreach (Formation formation in formations)
            {
                if (GUILayout.Button(formation.name))
                {
                    prefab = new GameObject("___Formation", typeof(Formation));
                    formationName = formation.name.Substring(FormationPrefix.Length, formation.name.Length - FormationPrefix.Length);
                    foreach (Vector3 pos in formation.positions)
                    {
                        var placeholder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        placeholder.transform.parent = prefab.transform;
                        placeholder.transform.localPosition = pos;
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
                oldFormation = formations.FirstOrDefault(p => p.name == FormationPrefix + formationName);
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
            Debug.Log("Deleting: " + Formation.FormationPath + FormationPrefix + oldFormation.name);
            AssetDatabase.DeleteAsset(Formation.FormationPath + FormationPrefix + oldFormation.name + ".prefab");
        }

        var savedPrefab = new GameObject(FormationPrefix + formationName, typeof(Formation));
        var formationComp = savedPrefab.GetComponent<Formation>();
        formationComp.positions = new Vector3[prefab.transform.childCount];
        for (int i = 0; i < formationComp.positions.Length; i++)
        {
            savedPrefab.GetComponent<Formation>().positions[i] = prefab.transform.GetChild(i).localPosition;
        }
        PrefabUtility.CreatePrefab(Formation.FormationPath + FormationPrefix + formationName + ".prefab", savedPrefab);
        DestroyImmediate(savedPrefab);

        formations = Formation.AllFormations();
    }

    #endregion
}