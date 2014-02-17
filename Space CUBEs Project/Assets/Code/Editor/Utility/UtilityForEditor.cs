// Steve Yeager
// 1.12.2014

using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;

public static class UtilityForEditor
{
    #region Const Fields

    private const string ENEMYPATH = "Assets/Ship/Characters/";

    #endregion


    #region Pubic Methods

    public static Dictionary<Enemy.Classes, GameObject> GetEnemyPrefabs()
    {
        string[] patternFiles = Directory.GetFiles(ENEMYPATH);
        Dictionary<Enemy.Classes, GameObject> enemies = new Dictionary<Enemy.Classes, GameObject>();
        for (int i = 0; i < patternFiles.Length; i++)
        {
            Object enemy = AssetDatabase.LoadAssetAtPath(patternFiles[i], typeof(GameObject));
            if (enemy != null && PrefabUtility.GetPrefabType(enemy) == PrefabType.Prefab)
            {
                GameObject enemyGO = enemy as GameObject;
                enemies.Add(enemyGO.GetComponent<Enemy>().enemyClass, enemyGO);
            }
        }
        return enemies;
    }
    
    #endregion
}