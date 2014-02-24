// Steve Yeager
// 11.26.2013

using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(CUBE))]
public class CUBEEditor : Editor
{
    #region Private Fields

    private CUBEInfo info;

    #endregion

    #region Const Fields

    private const string PREFIX = "CUBE ";

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            CUBEInfo[] cubes = CUBE.LoadAllCUBEInfo();
            info = cubes.First(c => c.name == target.name);
        }
    }


    public override void OnInspectorGUI()
    {
        // stats
        General();
        if (info.type == CUBE.Types.System)
        {
            System();
        }
        else if (info.type == CUBE.Types.Augmentation)
        {
            Augmentation();
        }
        Stats();
    }

    #endregion

    #region Private Methods

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


    private void Augmentation()
    {
        EditorGUILayout.LabelField("Limit", info.limit.ToString());
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
}