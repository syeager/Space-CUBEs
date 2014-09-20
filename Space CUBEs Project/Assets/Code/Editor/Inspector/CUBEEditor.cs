// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.26
// Edited: 2014.09.20

using System.Linq;
using UnityEditor;

[CustomEditor(typeof(CUBE))]
public class CUBEEditor : Editor
{
    #region Private Fields

    private CUBEInfo info;

    #endregion

    #region Editor Overrides

    public override void OnInspectorGUI()
    {
        LoadInfo();

        // stats
        General();
        if (info.type == CUBE.Types.System)
        {
            System();
        }

        Stats();
    }

    #endregion

    #region GUI Methods

    private void General()
    {
        EditorGUILayout.LabelField("ID", serializedObject.FindProperty("ID").intValue.ToString());
        EditorGUILayout.LabelField("Type", info.type.ToString());
    }


    private void System()
    {
        EditorGUILayout.LabelField("Subsystem", info.subsystem.ToString());
        EditorGUILayout.LabelField("Brand", info.brand.ToString());
        EditorGUILayout.LabelField("Grade", info.grade.ToString());
    }


    private void Stats()
    {
        EditorGUILayout.LabelField("Health", info.health.ToString());
        EditorGUILayout.LabelField("Shield", info.shield.ToString());
        EditorGUILayout.LabelField("Speed", info.speed.ToString());
        EditorGUILayout.LabelField("Damage", info.damage.ToString());
        EditorGUILayout.LabelField("Size", info.size.ToString());
        EditorGUILayout.LabelField("Cost", info.cost.ToString());
        EditorGUILayout.LabelField("Rarity", info.rarity.ToString());
        EditorGUILayout.LabelField("Price", info.price.ToString());
    }

    #endregion

    #region Private Methods

    private void LoadInfo()
    {
        if (info == null)
        {
            CUBEInfo[] cubes = CUBE.AllCUBES ?? CUBE.LoadAllCUBEInfo();
            info = cubes.First(c => c.name == target.name);
        }
    }

    #endregion
}