// Steve Yeager
// 11.26.2013

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CUBE))]
public class CUBEEditor : Editor
{
    #region Serialized Fields

    private SerializedObject sObject;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        sObject = new SerializedObject(target);
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        sObject.Update();

        EditorGUILayout.Space();


        sObject.ApplyModifiedProperties();
    }

    #endregion
}