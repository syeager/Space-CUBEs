// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.19
// Edited: 2014.10.19

using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FindReferences
{
    [MenuItem("CONTEXT/MonoBehaviour/Find In Scene")]
    public static void FindReferencesInScene(MenuCommand command)
    {
        MonoBehaviour script = (MonoBehaviour)command.context;

        Selection.objects = Resources.FindObjectsOfTypeAll(script.GetType()).Where(obj => !AssetDatabase.Contains(obj)).ToArray();
    }
}