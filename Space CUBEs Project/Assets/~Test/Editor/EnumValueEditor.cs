// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.16
// Edited: 2014.09.16

using System.Linq;
using UnityEditor;

[CustomEditor(typeof(EnumValue))]
public class EnumValueEditor : Editor
{
    private System.Type[] enums;
    private int typeIndex;

    private void OnEnable()
    {
        EnumValue enumValue = (EnumValue)target;
        enums = System.Reflection.Assembly.GetAssembly(typeof(EnumValue.EmptyEnum)).GetTypes().Where(t => t.IsEnum).ToArray();
        typeIndex = System.Array.IndexOf(enums, enumValue.enumType);
        if (typeIndex == -1) typeIndex = 0;
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnumValue enumValue = (EnumValue)target;
        enumValue.CheckValue();

        typeIndex = EditorGUILayout.Popup(typeIndex, enums.Select(e => e.Name).ToArray());
        System.Type type = enums[typeIndex];
        if (type != enumValue.enumType)
        {
            enumValue.enumType = type;
            enumValue.value = 0;
        }

        enumValue.value = EditorGUILayout.Popup(enumValue.value, System.Enum.GetNames(type));

        serializedObject.ApplyModifiedProperties();
    }
}