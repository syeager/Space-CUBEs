// Steve Yeager
// 8.18.2013

using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System.Collections.Generic;
using System.Collections;

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

    public bool overwrite;
    public bool[] logFlags =
    {
        true,    // Default
        true,    // Data
    };
    public enum LogTypes
    {
        Default = 0,
        Data = 1,
    }

    #endregion

    #region Private Fields

    private static Queue<KeyValuePair<string, float>> messages = new Queue<KeyValuePair<string, float>>();
    private static bool displayingMessages;

    #endregion

    #region Const Fields

    private const string LOGPATH = "/Files/Debug Logs/Log_";

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        string[] logTypes = Enum.GetNames(typeof (LogTypes));
        for (int i = 0; i < logFlags.Length; i++)
        {
            string path = Application.dataPath + LOGPATH + logTypes[i] + ".txt";
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

        // clear ConsoleLine
        if (ConsoleLine != null) ConsoleLine.guiText.text = "";
    }

    #endregion

    #region Log/Warning/Error Methods

    [System.Diagnostics.Conditional("LOG")]
    new public static void Log(object message, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int) logType]) Debug.Log(message);

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "\r\n" + message);
            }
        }
    }


    [System.Diagnostics.Conditional("LOG")]
    public static void Log(object message, Object context, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.Log(message, context);

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "\r\n" + message);
            }
        }
    }


    [System.Diagnostics.Conditional("WARNING")]
    new public static void LogWarning(object message, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.LogWarning(message);

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + " Warning\r\n" + message);
            }
        }
    }


    [System.Diagnostics.Conditional("WARNING")]
    public static void LogWarning(object message, Object context, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.LogWarning(message, context);

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "Warning\r\n" + message);
            }
        }
    }


    [System.Diagnostics.Conditional("ERROR")]
    new public static void LogError(object message, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.LogError(message);

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "Error\r\n" + message);
            }
        }
    }


    [System.Diagnostics.Conditional("ERROR")]
    public static void LogError(object message, Object context, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Main.logFlags[(int)logType]) Debug.LogError(message, context);

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine("Time: " + Time.realtimeSinceStartup + "Error\r\n" + message);
            }
        }
    }

    #endregion

    #region ConsoleLine Messages

    [System.Diagnostics.Conditional("CONSOLELINE")]
    public static void LogConsoleLine(string message, float time = 0f)
    {
        if (Main.ConsoleLine == null) return;

        messages.Enqueue(new KeyValuePair<string, float>(message, time));
        if (!displayingMessages)
        {
            Main.StartCoroutine(DisplayConsoleLine());
        }
    }


    private static IEnumerator DisplayConsoleLine()
    {
        displayingMessages = true;
        while (messages.Count > 0)
        {
            var message = messages.Dequeue();
            Main.ConsoleLine.guiText.text = message.Key;
            yield return new WaitForSeconds(message.Value);
        }

        Main.ConsoleLine.guiText.text = "";
        displayingMessages = false;
    }

    #endregion
}