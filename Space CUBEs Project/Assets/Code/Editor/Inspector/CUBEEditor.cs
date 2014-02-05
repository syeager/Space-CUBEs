// Steve Yeager
// 11.26.2013

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CUBE))]
public class CUBEEditor : Editor
{
    #region Serialized Fields

    private SerializedProperty ID;

    #endregion

    #region Private Fields

    private CUBEInfo[] CUBEInfos;
    private CUBEInfo info;

    #endregion

    #region Const Fields

    private const string PREFIX = "CUBE ";

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        CUBEInfos = CUBE.LoadAllCUBEInfo();

        ID = serializedObject.FindProperty("ID");

        if (ID.intValue > -1)
        {
            LoadInfo(ID.intValue);
        }
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // id not set
        if (ID.intValue == -1)
        {
            EditorGUILayout.LabelField("ID not set.");
        }
        // show info
        else
        {
            // ID
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(ID);
                ID.intValue = Mathf.Clamp(ID.intValue, 0, CUBEInfos.Length-1);
            }
            if (EditorGUI.EndChangeCheck())
            {
                LoadInfo(ID.intValue);
            }

            EditorGUILayout.LabelField("Type", info.type.ToString());
            EditorGUILayout.LabelField("Health", info.health.ToString());
            EditorGUILayout.LabelField("Shield", info.shield.ToString());
            EditorGUILayout.LabelField("Speed", info.speed.ToString());
            EditorGUILayout.LabelField("Rarity", info.rarity.ToString());
            EditorGUILayout.LabelField("Price", info.price.ToString());
        }

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void LoadInfo(int ID)
    {
        info = CUBEInfos[ID];
        (target as CUBE).name = PREFIX + info.name;
        PrefabUtility.GetPrefabObject((target as CUBE).gameObject).name = PREFIX + info.name;
        serializedObject.FindProperty("type").enumValueIndex = (int)info.type;
        serializedObject.FindProperty("health").floatValue = info.health;
        serializedObject.FindProperty("shield").floatValue = info.shield;
        serializedObject.FindProperty("speed").floatValue = info.speed;
        serializedObject.FindProperty("rarity").intValue = info.rarity;
        serializedObject.FindProperty("price").intValue = info.price;
    }


    private void CombatStats()
    {

    }


    private void PartStats()
    {

    }


    private void SystemStats()
    {

    }


    private void AugmentationStats()
    {

    }

    #endregion
}