// Steve Yeager
// 2.23.2014

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Diagnostics;

/// <summary>
/// 
/// </summary>
public class ColorSaver : EditorWindow
{
    #region Static Fields

    private static UIGrid grid;

    #endregion

    #region Readonly Fields

    private static readonly Vector2 SIZE = new Vector2(300f, 50f);    
    
    #endregion

    #region Const Fields

    private const string COLORSELECTOR = "Color Selector";
    private static readonly string COLORLISTPATHEDITOR = Application.dataPath + "/Resources/" + CUBE.COLORLIST + ".bytes";

    #endregion


    #region EditorWindow Overrides

    [MenuItem("Tools/Save Colors", true)]
    public static bool Validate()
    {
        return EditorApplication.currentScene == "Assets/Levels/Garage.unity";
    }


    [MenuItem("Tools/Save Colors")]
    public static void Init()
    {
        ColorSaver window = (ColorSaver)EditorWindow.GetWindow<ColorSaver>(true, "Save Colors");
        window.minSize = window.maxSize = SIZE;

        grid = GameObject.Find(COLORSELECTOR).GetComponentInChildren<UIGrid>();
        if (grid == null)
        {
            UnityEngine.Debug.LogError("Grid not found!");
        }
    }


    private void OnGUI()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save Colors"))
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            SaveColors();
            UnityEngine.Debug.Log("Colors saved in: " + watch.ElapsedMilliseconds + " ms");
        }
    }

    #endregion

    #region Private Methods

    public static void SaveColors()
    {
        UISprite[] sprites = grid.GetComponentsInChildren<UISprite>(true);
        Color[] colors = new Color[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            colors[i] = sprites[i].color;
            sprites[i].GetComponent<ActivateButton>().value = i.ToString();
        }

        using (BinaryWriter writer = new BinaryWriter(File.Open(COLORLISTPATHEDITOR, FileMode.Create)))
        {
            foreach (var color in colors)
            {
                writer.Write(color.ToString());
            }
        }
    }

    #endregion
}