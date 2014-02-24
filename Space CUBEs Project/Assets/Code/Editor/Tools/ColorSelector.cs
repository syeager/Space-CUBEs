// Steve Yeager
// 2.23.2014

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 
/// </summary>
public class ColorSelector : EditorWindow
{
    #region Static Fields
    
    private static Color[] colors;
    private static ColorVertices cv;
    private static int piece;
    private static readonly Vector2 SIZE = new Vector2(820f, 420f);
    private const float COLORSIZE = 100f;
    
    #endregion


    #region Static Methods

    public static void OpenSelector(Color[] colors, ColorVertices cv, int piece)
    {
        ColorSelector window = EditorWindow.GetWindow<ColorSelector>(true, "Color Selector") as ColorSelector;
        window.minSize = SIZE;
        window.maxSize = SIZE;

        ColorSelector.colors = colors;
        ColorSelector.cv = cv;
        ColorSelector.piece = piece;
    }


    private void OnGUI()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                GUI.backgroundColor = colors[(r * 8) + (c)];
                if (GUI.Button(new Rect(10 + c*COLORSIZE, 10 + r*COLORSIZE, COLORSIZE, COLORSIZE), ""))
                {
                    SerializedObject so = new SerializedObject(cv);
                    so.Update();
                    cv.SetandBake(piece, colors[(r * 8) + (c)]);
                    so.ApplyModifiedProperties();
                    Close();
                }
            }
        }
    }

    #endregion
}