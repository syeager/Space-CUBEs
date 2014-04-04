// Steve Yeager
// 4.3.2014

using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CustomEditor(typeof(ActivateButton))]
public class ActivateButtonEditor : UIButtonEditor
{
    #region Editor Overrides

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ActivateButton button = (target as ActivateButton);
        bool toggled = false;
        EditorGUI.BeginChangeCheck();
        {
            toggled = EditorGUILayout.Toggle("Enabled", button.isEnabled);
        }
        if (EditorGUI.EndChangeCheck())
        {
            button.isEnabled = toggled;
            button.UpdateColor(toggled, true);
        }

        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));

        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}