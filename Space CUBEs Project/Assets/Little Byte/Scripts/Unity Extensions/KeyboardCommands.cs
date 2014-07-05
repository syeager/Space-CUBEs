// Steve Yeager
// 4.26.2014

using System;
using UnityEngine;
using Annotations;

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