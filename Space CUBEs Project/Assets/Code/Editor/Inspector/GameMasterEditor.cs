// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.27
// Edited: 2014.05.27

using UnityEditor;

/// <summary>
/// Sets target FPS.
/// </summary>
[CustomEditor(typeof(GameMaster))]
public class GameMasterEditor : Creator<GameMaster>
{
    #region Editor Overrides

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int targetFPS = EditorGUILayout.IntSlider("Target FPS", GameTime.targetFPS, GameTime.MinFPS, GameTime.MaxFPS);
        if (targetFPS != GameTime.targetFPS)
        {
            GameTime.CapFPS(targetFPS);
        }

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Creator Overrides

    [MenuItem("GameObject/Singletons/Game Master", false, 4)]
    public static void Create()
    {
        Create("__GameMaster");
    } 

    #endregion
}