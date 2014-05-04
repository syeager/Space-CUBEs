//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

#if UNITY_3_5
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIGrid))]
public class UIGridEditor : UIWidgetContainerEditor
{
}
#else
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIGrid))]
public class UIGridEditor : UIWidgetContainerEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Reposition"))
        {
            (target as UIGrid).Reposition();
        }
    }
}
#endif
