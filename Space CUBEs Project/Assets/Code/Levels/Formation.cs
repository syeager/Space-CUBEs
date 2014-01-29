// Steve Yeager
// 1.12.2014

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Formation : MonoBehaviour
{
    #region Public Fields

    public Vector3[] positions;

    #endregion

    #region Const Fields

    public const string FORMATIONPATH = "Assets/Formations/";

    #endregion


    #region Static Methods

    #if UNITY_EDITOR
    public static Formation[] AllFormations()
    {
        string[] formationFiles = System.IO.Directory.GetFiles(Formation.FORMATIONPATH);
        List<Formation> formations = new List<Formation>();

        for (int i = 0; i < formationFiles.Length; i++)
        {
            UnityEngine.Object formation = UnityEditor.AssetDatabase.LoadAssetAtPath(formationFiles[i], typeof(GameObject));
            if (formation != null && UnityEditor.PrefabUtility.GetPrefabType(formation) == UnityEditor.PrefabType.Prefab)
            {
                formations.Add((formation as GameObject).GetComponent<Formation>());
            }
        }

        return formations.ToArray();
    }
    #endif

    #endregion
}