// Little Byte Games
// Author: Steve Yeager
// Created: 2014.04.27
// Edited: 2014.09.17

using System;
using UnityEngine;
using Annotations;
using LittleByte;

/// <summary>
/// Script to put keyboard shortcuts that aid in debugging.
/// </summary>
public class KeyboardCommands : MonoBehaviour
{
#if UNITY_EDITOR
    [UsedImplicitly]
    private void Update()
    {
        // reload level
        if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            SceneManager.ReloadScene();
        }

        // clear console
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            System.Reflection.MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
#endif
}