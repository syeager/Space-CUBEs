// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.16
// Edited: 2014.09.16

using UnityEditor;

namespace SpaceCUBEs
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GarageMenuButton))]
    public class GarageMenuButtonEditor : UIButtonEditor
    {
        #region Editor Override

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("menu"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("closeY"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("openY"));
            
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        #endregion
    } 
}