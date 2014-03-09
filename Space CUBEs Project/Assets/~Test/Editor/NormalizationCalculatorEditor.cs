// Steve Yeager
// 3.8.2014

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(NormalizationCalculator))]
public class NormalizationCalculatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Calculate"))
        {
            (target as NormalizationCalculator).Normalize();
        }
    }
}