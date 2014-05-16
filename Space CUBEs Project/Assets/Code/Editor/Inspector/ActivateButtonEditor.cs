// Steve Yeager
// 4.3.2014

using UnityEditor;

/// <summary>
/// Editor for ActivateButton.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(ActivateButton), true)]
public class ActivateButtonEditor : UIButtonEditor
{
    #region Editor Overrides

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ActivateButton button = (target as ActivateButton);
        bool toggled;
        EditorGUI.BeginChangeCheck();
        {
            toggled = EditorGUILayout.Toggle("Enabled", button.isEnabled);
        }
        if (EditorGUI.EndChangeCheck())
        {
            button.Toggle(toggled);
        }

        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("label"));

        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}