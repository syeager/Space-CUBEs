// Steve Yeager
// 1.10.2014

using UnityEditor;
using System.IO;
using UnityEngine;

public class WaveCreator : EditorWindow
{
    #region Private Fields

    private int level;
    private int wave = 1;
    private bool confirmOverwrite;

    #endregion

    #region Static Fields

    private static string[] levels;
    private static readonly Vector2 SIZE = new Vector2(300f, 50f);

    #endregion

    #region Const Fields

    private const string LEVELPATH = "Assets/Levels/";
    
    #endregion
    

    #region EditorWindow Overrides

    [MenuItem("Tools/Create Wave")]
    private static void Init()
    {
        WaveCreator window = (WaveCreator)EditorWindow.GetWindow(typeof(WaveCreator), true, "Wave Creator");
        window.minSize = SIZE;
        window.maxSize = SIZE;
    }


    private void OnEnable()
    {
        levels = Directory.GetFiles(LEVELPATH, "*.unity");
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = levels[i].Replace(LEVELPATH, "");
            levels[i] = levels[i].Replace(".unity", "");
        }
    }


    private void OnGUI()
    {
        // level and wave
        EditorGUILayout.BeginHorizontal();
        {
            level = EditorGUILayout.Popup(level, levels, GUILayout.MaxWidth(150f));
            EditorGUILayout.LabelField("Wave ", GUILayout.MaxWidth(50f));
            wave = EditorGUILayout.IntField(wave, GUILayout.MaxWidth(25f));
            if (GUILayout.Button("-", EditorStyles.miniButtonLeft))
            {
                wave--;
            }
            if (GUILayout.Button("+", EditorStyles.miniButtonRight))
            {
                wave++;
            }
            if (wave < 1) wave = 1;

        }
        EditorGUILayout.EndHorizontal();

        // clear and load/save
        EditorGUILayout.BeginHorizontal();
        {
            if (confirmOverwrite)
            {
                if (GUILayout.Button("Cancel"))
                {
                    confirmOverwrite = false;
                }
                if (GUILayout.Button("Overwrite"))
                {
                    WaveStream.Write(levels[level], wave, GameObject.FindObjectsOfType<Enemy>(), true);
                    confirmOverwrite = false;
                }
            }
            else
            {
                if (GUILayout.Button("Load"))
                {
                    var enemies = WaveStream.Read(levels[level], wave);
                    if (enemies == null)
                    {
                        Debug.LogWarning(levels[level] + " Wave " + wave + " does not exist.");
                    }
                    else
                    {
                        LoadWave(enemies);
                    }
                }
                if (GUILayout.Button("Save"))
                {
                    if (WaveStream.Write(levels[level], wave, GameObject.FindObjectsOfType<Enemy>(), false))
                    {
                        confirmOverwrite = true;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Private Methods

    private void LoadWave(WaveEnemyData[] enemies)
    {
        foreach (var enemyData in enemies)
        {
            // need a way to load enemies without the PoolManager.
            Debug.Log(enemyData.enemy.ToString());
            //GameObject enemy = (GameObject)GameObject.Instantiate(PoolManager.GetPrefab(enemyData.enemy.ToString()));
            //enemy.transform.position = enemyData.position;
        }
    }

    #endregion
}