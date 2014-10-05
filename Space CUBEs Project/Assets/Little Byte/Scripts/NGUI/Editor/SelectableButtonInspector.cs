// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.09.01

using UnityEditor;

namespace LittleByte.NGUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SelectableButton))]
    public class SelectableButtonInspector : ActivateButtonEditor
    {
        #region Editor Overrides

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startSelected"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggle"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("activateType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dragScrollView"));

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        #endregion
    } 
}