// Steve Yeager
// 5.22.2014

using System.IO;
using Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Keyboard shortcut to open file or asset in explorer.
/// </summary>
public class ShowInExplorer : EditorWindow
{
    #region EditorWindow Overrides

    [UsedImplicitly]
    [MenuItem("Assets/Show in Explorer %E")]
    private static void Init()
    {
        string assetsPath = Application.dataPath.Remove(Application.dataPath.Length - 7) + @"\";
        foreach (var selectedObject in Selection.objects)
        {
            bool openInsidesOfFolder = false;
            string path = (assetsPath + AssetDatabase.GetAssetPath(selectedObject)).Replace(@"/", @"\");
            Debugger.Log("Opening: " + path, selectedObject);
            if (Directory.Exists(path))
            {
                openInsidesOfFolder = true;
            }
            System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + path);
        }
    }
    
    #endregion
}