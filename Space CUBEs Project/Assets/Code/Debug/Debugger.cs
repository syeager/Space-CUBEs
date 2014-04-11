// Steve Yeager
// 8.18.2013

using System;
using Annotations;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

using Object = UnityEngine.Object;

/// <summary>
/// Wrapper for Unity's Debug class.
/// </summary>
public class Debugger : Singleton<Debugger>
{
    #region References

    public GameObject FPS;
    public GameObject FPS_Prefab;
    public GameObject ConsoleLine;
    public GameObject ConsoleLine_Prefab;

    #endregion

    #region Public Fields

    public bool showTime;
    public bool overwrite;
    public bool[] logFlags =
    {
        true,    // Default
        true,    // Data
        true,    // LevelEvents
        true,    // StateMachines
        true,    // Construction
    };
    public enum LogTypes
    {
        Default = 0,
        Data = 1,
        LevelEvents = 2,
        StateMachines = 3,
        Construction = 4,
    }

    #endregion

    #region Private Fields

    private static readonly Queue<KeyValuePair<string, float>> Messages = new Queue<KeyValuePair<string, float>>();
    private static bool displayingMessages;

    #endregion

    #region Const Fields

    private const string LogPath = "/Files/Debug Logs/Log_";

    #endregion


    #region MonoBehaviour Overrides

    [System.Diagnostics.Conditional("LOG")]
    private void Start()
    {
        string[] logTypes = Enum.GetNames(typeof(LogTypes));
        for (int i = 0; i < logFlags.Length; i++)
        {
            string path = Application.dataPath + LogPath + logTypes[i] + ".txt";
            // clear file
            if (overwrite)
            {
                File.WriteAllText(path, String.Empty);
            }
            // append to file
            else
            {
                File.AppendAllText(path, "\r\n");
            }

            File.AppendAllText(path, "*** " + DateTime.Now + " ***\r\n");
        }
    }

#if UNITY_EDITOR
    [UsedImplicitly]
    private void Update()
    {
        // reload level
        if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            GameData.ReloadLevel();
        }

        // clear console
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
#endif

    #endregion

    #region Log/Warning/Error Methods

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(object message, Object context = null, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.Log((Main.showTime ? Time.time + " " : "") + message, context);

#if UNITY_EDITOR
        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LogPath + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "\r\n" + message);
            }
        }
#endif
    }


    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogWarning(object message, Object context = null, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.LogWarning((Main.showTime ? Time.time + " " : "") + message, context);

#if UNITY_EDITOR
        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LogPath + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "Warning\r\n" + message);
            }
        }
#endif
    }


    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogError(object message, Object context = null, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.LogError((Main.showTime ? Time.time + " " : "") + message, context);

#if UNITY_EDITOR
        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LogPath + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "Error\r\n" + message);
            }
        }
#endif
    }


    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogList(IEnumerable list, string header = "", Object context = null, LogTypes logType = LogTypes.Default)
    {
        if (!Main.logFlags[(int)logType]) return;

        if (header != "")
        {
            Debug.Log(header + '\n', context);
        }

        int count = 0;
        foreach (var item in list)
        {
            Debug.Log(count + ": " + item, context);
            count++;
        }

        if (count == 0)
        {
            Debug.Log("Empty");
        }
    }


    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogFields(object context, string name, bool includePrivate = false, LogTypes logType = LogTypes.Default)
    {
        if (!Main.logFlags[(int)logType]) return;

        Type type = context.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        if (includePrivate) flags |= BindingFlags.NonPublic;
        FieldInfo[] info = type.GetFields(flags);

        Debug.Log(name, (context is Object ? context as Object : null));
        foreach (var i in info)
        {
            object o = i.GetValue(context);
            Debug.Log(i.Name + ": " + o, (o is Object ? o as Object : null));
        }
    }

    #endregion

    #region ConsoleLine Messages

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogConsoleLine(string message, float time = 0f)
    {
        if (Main.ConsoleLine == null) return;

        Messages.Enqueue(new KeyValuePair<string, float>(message, time));
        if (!displayingMessages)
        {
            Main.StartCoroutine(DisplayConsoleLine());
        }
    }


    private static IEnumerator DisplayConsoleLine()
    {
        displayingMessages = true;
        while (Messages.Count > 0)
        {
            var message = Messages.Dequeue();
            Main.ConsoleLine.guiText.text = message.Key;
            yield return new WaitForSeconds(message.Value);
        }

        Main.ConsoleLine.guiText.text = "";
        displayingMessages = false;
    }

    #endregion
}