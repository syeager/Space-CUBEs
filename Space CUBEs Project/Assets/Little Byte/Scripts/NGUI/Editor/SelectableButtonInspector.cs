// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.09.01

using UnityEditor;

namespace LittleByte.NGUI
{
    [CustomEditor(typeof(SelectableButton))]
    public class SelectableButtonInspector : UIButtonEditor
    {
        #region Editor Overrides

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group"));

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    } 
}