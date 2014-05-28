// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.02.23
// Edited: 2014.05.23

using System.Diagnostics;
using System.IO;
using Annotations;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Save current color pallet.
/// </summary>
public class ColorSaver : EditorWindow
{
    #region Static Fields

    private static UIGrid grid;

    #endregion

    #region Readonly Fields

    private static readonly Vector2 Size = new Vector2(300f, 50f);

    #endregion

    #region Const Fields

    private const string ColorSelector = "Color Selector";
    private static readonly string ColorListPathEditor = Application.dataPath + "/Resources/" + CUBE.COLORLIST + ".bytes";

    #endregion

    #region EditorWindow Overrides

    [MenuItem("CUBEs/Save Colors", true, 1)]
    public static bool Validate()
    {
        return EditorApplication.currentScene.Contains("Garage");
    }


    [MenuItem("CUBEs/Save Colors", false, 1)]
    public static void Init()
    {
        var window = GetWindow<ColorSaver>(true, "Save Colors");
        window.minSize = window.maxSize = Size;

        grid = GameObject.Find(ColorSelector).GetComponentInChildren<UIGrid>();
        if (grid == null)
        {
            Debug.LogError("Grid not found!");
        }
    }


    [UsedImplicitly]
    private void OnGUI()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save Colors"))
        {
            var watch = new Stopwatch();
            watch.Start();
            SaveColors();
            Debug.Log("Colors saved in: " + watch.ElapsedMilliseconds + " ms");
        }
    }

    #endregion

    #region Private Methods

    public static void SaveColors()
    {
        UISprite[] sprites = grid.GetComponentsInChildren<UISprite>(true);
        var colors = new Color[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            colors[i] = sprites[i].color;
            sprites[i].GetComponent<ActivateButton>().value = i.ToString();
        }

        using (var writer = new BinaryWriter(File.Open(ColorListPathEditor, FileMode.Create)))
        {
            foreach (Color color in colors)
            {
                writer.Write(color.ToString());
            }
        }
    }

    #endregion
}