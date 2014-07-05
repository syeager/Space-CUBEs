// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.15
// Edited: 2014.06.29

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Pool))]
public class PoolDrawer : PropertyDrawer
{
    #region Private Fields

    private SerializedProperty prefab;
    private SerializedProperty preAllocate;
    private SerializedProperty allocateBlock;
    private SerializedProperty hardLimit;
    private SerializedProperty limit;
    private SerializedProperty cull;
    private SerializedProperty cullLimit;
    private SerializedProperty parent;

    #endregion

    #region Const Fields

    private const float PropertyHeight = 18f;

    #endregion

    #region PropertyDrawer Overrides

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CacheProperties(property);
        position.height = PropertyHeight;
        Color cachedColor = GUI.color;

        // label
        Object prefabRef = prefab.objectReferenceValue;
        string prefabName;
        if (prefabRef == null)
        {
            GUI.color = Color.red;
            prefabName = "EMPTY";
        }
        else
        {
            prefabName = prefabRef.name;
        }
        
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, prefabName);

        if (property.isExpanded)
        {
            position.y += PropertyHeight;
            EditorGUI.PropertyField(position, prefab);
            GUI.color = cachedColor;
            position.y += PropertyHeight;
            EditorGUI.PropertyField(position, preAllocate);
            position.y += PropertyHeight;
            EditorGUI.PropertyField(position, allocateBlock);
            position.y += PropertyHeight;
            EditorGUI.PropertyField(position, hardLimit);
            if (hardLimit.boolValue)
            {
                position.y += PropertyHeight;
                EditorGUI.PropertyField(position, limit);
            }
            position.y += PropertyHeight;
            EditorGUI.PropertyField(position, cull);
            if (cull.boolValue)
            {
                position.y += PropertyHeight;
                EditorGUI.PropertyField(position, cullLimit);
            }
            position.y += PropertyHeight;
            EditorGUI.PropertyField(position, parent);
        }
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return PropertyHeight;

        int properties = 7;
        if (property.FindPropertyRelative("hardLimit").boolValue) properties++;
        if (property.FindPropertyRelative("cull").boolValue) properties++;

        return properties * PropertyHeight;
    }

    #endregion

    #region Private Methods

    private void CacheProperties(SerializedProperty property)
    {
        prefab = property.FindPropertyRelative("prefab");
        preAllocate = property.FindPropertyRelative("preAllocate");
        allocateBlock = property.FindPropertyRelative("allocateBlock");
        hardLimit = property.FindPropertyRelative("hardLimit");
        limit = property.FindPropertyRelative("limit");
        cull = property.FindPropertyRelative("cull");
        cullLimit = property.FindPropertyRelative("cullLimit");
        parent = property.FindPropertyRelative("parent");
    }

    #endregion
}