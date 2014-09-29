// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.23
// Edited: 2014.09.23

using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ScrollviewButton), true)]
public class ScrollviewButtonEditor : ActivateButtonEditor
{
    #region Editor Overrides

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
       
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dragScrollView"));

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }

    #endregion
}