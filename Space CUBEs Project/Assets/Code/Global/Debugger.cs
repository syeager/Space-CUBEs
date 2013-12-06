// Steve Yeager
// 8.18.2013

using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;

/// <summary>
/// Wrapper for Unity's Debug class.
/// </summary>
public class Debugger : MonoBehaviour
{
    #region References

    public GameObject FPS;
    public GameObject FPS_Prefab;

    #endregion

    #region Public Fields

    public static Debugger Main;
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

    #region Const Fields

    private const string LOGPATH = "/Files/Debug/Log_";

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // singleton
        Main = this;

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
    }

    #endregion

    #region Static Methods

    [System.Diagnostics.Conditional("LOG")]
    public static void Log(object message, bool save = true, LogTypes logType = LogTypes.Default)
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
    public static void LogWarning(object message, bool save = true, LogTypes logType = LogTypes.Default)
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
    public static void LogError(object message, bool save = true, LogTypes logType = LogTypes.Default)
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
}