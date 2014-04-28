// Steve Yeager
// 8.18.2013

using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Annotations;

using Object = UnityEngine.Object;

/// <summary>
/// Wrapper for Unity's Debug class.
/// </summary>
public class Debugger : Singleton<Debugger>
{
    #region References

    public GameObject fps;
    public GameObject fpsPrefab;
    public GameObject consoleLine;
    public GameObject consoleLinePrefab;

    #endregion

    #region Public Fields

    /// <summary>Should the time since game started be logged?</summary>
    public bool showTime;

    /// <summary>Different methods of saving logs to files.</summary>
    public enum LogSaving
    {
        DontSave,
        Save,
        SaveAndClear,
    }

    /// <summary>Current saving method.</summary>
    public LogSaving logSaving = LogSaving.Save;

    /// <summary>Flags for LogTypes. Only checked flags will have their corresponding LogTypes logged.</summary>
    public bool[] logFlags =
    {
        true,    // Default
        true,    // Data
        true,    // LevelEvents
        true,    // StateMachines
        true,    // Construction
    };

    /// <summary>Catagorization for logging. Can be added to but need to update logFlags as well.</summary>
    public enum LogTypes
    {
        Default = 0,
        Data = 1,
        LevelEvents = 2,
        StateMachines = 3,
        Construction = 4,
    }

#if UNITY_EDITOR
    /// <summary>Should the Editor pause if below 10 FPS?</summary>
    public bool lowBreak = true;
#endif

    #endregion

    #region Private Fields

    /// <summary>Time in seconds accumulated over current FPS interval.</summary>
    private float accum;

    /// <summary>Number of frames drawn in the current interval.</summary>
    private int frames;

    /// <summary>Time in seconds left in the current interval.</summary>
    private float timeleft = UpdateInterval;

    #endregion

    #region Static Fields

    /// <summary>Are messages currently being displayed to the ConsoleLine?</summary>
    private static bool displayingMessages;

    /// <summary>Current marker for Debug.Mark.</summary>
    private static int marker;

    #endregion

    #region Readonly Fields

    // TODO: Move file to resources folder to work on builds?
    private static readonly string LogPath = Application.dataPath + "/Files/Debug Logs/Log_";

    /// <summary>Messages queued up for the ConsoleLine and their time in seconds to be displayed.</summary>
    private static readonly Queue<KeyValuePair<string, float>> Messages = new Queue<KeyValuePair<string, float>>();

    #endregion

    #region Const Fields

    private const string FileExt = ".txt";

    /// <summary>Time in seconds to get average FPS.</summary>
    private const float UpdateInterval = 0.5f;

    #endregion


    #region MonoBehaviour Overrides

#if UNITY_EDITOR
    /// <summary>
    /// Set up log files.
    /// </summary>
    [UsedImplicitly]
    private void Start()
    {
        if (logSaving == LogSaving.DontSave) return;

        string[] logTypes = Enum.GetNames(typeof(LogTypes));
        for (int i = 0; i < logFlags.Length; i++)
        {
            string path = LogPath + logTypes[i] + FileExt;
            // clear file
            if (logSaving == LogSaving.SaveAndClear)
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
#endif


    [UsedImplicitly]
    private void Update()
    {
        if (fps == null) return;

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            float average = accum / frames;
            fps.GetComponent<HUDFPS>().UpdateFPS(average);

#if UNITY_EDITOR
            if (lowBreak && average < 10)
            {
                Debug.Break();
            }
#endif

            timeleft = UpdateInterval;
            accum = 0f;
            frames = 0;
        }
    }


    /// <summary>
    /// Clean up FPS.
    /// </summary>
    [UsedImplicitly]
    private void OnDestroy()
    {
        if (fps != null)
        {
            Destroy(fps);
        }
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Displays a log message to the Console.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    /// <param name="save">Should this message be saved to a log file?</param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(object message, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        if (Main.logFlags[(int)logType]) Debug.Log((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + message, context);

#if UNITY_EDITOR
        if (save && Main.logSaving != LogSaving.DontSave)
        {
            using (StreamWriter writer = new StreamWriter(LogPath + logType + FileExt, true))
            {
                writer.WriteLine("[{0}] Log\r\n {1}", Time.realtimeSinceStartup, message);
            }
        }
#endif
    }


    /// <summary>
    /// Displays a warning message to the Console.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    /// <param name="save">Should this message be saved to a log file?</param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogWarning(object message, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        if (Main.logFlags[(int)logType]) Debug.LogWarning((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + message, context);

#if UNITY_EDITOR
        if (save && Main.logSaving != LogSaving.DontSave)
        {
            using (StreamWriter writer = new StreamWriter(LogPath + logType + FileExt, true))
            {
                writer.WriteLine("[{0}] Warning\r\n {1}", Time.realtimeSinceStartup, message);
            }
        }
#endif
    }


    /// <summary>
    /// Displays an error message to the Console.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Used for catagorizing the message.</param>
    /// <param name="save">Should this message be saved to a log file?</param>
    public static void LogError(object message, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        Debug.LogError((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + message, context);

#if UNITY_EDITOR
        if (save && Main.logSaving != LogSaving.DontSave)
        {
            using (StreamWriter writer = new StreamWriter(LogPath + logType + FileExt, true))
            {
                writer.WriteLine("[{0}] Error\r\n {1}", Time.realtimeSinceStartup, message);
            }
        }
#endif
    }


    /// <summary>
    /// Displays an exception message to the Console.
    /// </summary>
    /// <param name="exception">Exception to be thrown and shown in the Console.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Used for catagorizing the message.</param>
    /// <param name="save">Should this message be saved to a log file?</param>
    public static void LogException(Exception exception, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        Debug.LogException(exception, context);

#if UNITY_EDITOR
        if (save && Main.logSaving != LogSaving.DontSave)
        {
            using (StreamWriter writer = new StreamWriter(LogPath + logType + FileExt, true))
            {
                writer.WriteLine("[{0}] Exception\r\n {1}", Time.realtimeSinceStartup, exception);
            }
        }
#endif
    }


    /// <summary>
    /// Runs through a list and displays its contents to the Console.
    /// </summary>
    /// <param name="list">List to display.</param>
    /// <param name="header">Message to display before the list.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
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


    /// <summary>
    /// Runs through a dictionary and displays its Keys and Values to the Console.
    /// </summary>
    /// <param name="dictionary">Dictionary to display.</param>
    /// <param name="header">Message to display before the list.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogDict<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string header = "", Object context = null, LogTypes logType = LogTypes.Default)
    {
        if (!Main.logFlags[(int)logType]) return;

        if (header != "")
        {
            Debug.Log(header + '\n', context);
        }

        int count = 0;
        foreach (var item in dictionary)
        {
            Debug.Log(count + ": " + item.Key + ", " + item.Value, context);
            count++;
        }

        if (count == 0)
        {
            Debug.Log("Empty");
        }
    }


    /// <summary>
    /// Display an instance's fields to the Console.
    /// </summary>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="name">Displayed before the instance's fields. Used for reference.</param>
    /// <param name="includePrivate">Should private members be shown as well?</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogFields(object context, string name, bool includePrivate = false, LogTypes logType = LogTypes.Default)
    {
        if (!Main.logFlags[(int)logType]) return;

        Type type = context.GetType();
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
        if (includePrivate) flags |= System.Reflection.BindingFlags.NonPublic;
        System.Reflection.FieldInfo[] info = type.GetFields(flags);

        Debug.Log(name, (context is Object ? context as Object : null));
        foreach (var i in info)
        {
            object o = i.GetValue(context);
            Debug.Log(i.Name + ": " + o, (o is Object ? o as Object : null));
        }
    }


    /// <summary>
    /// Updates an internal counter and displays current value to the Console.
    /// </summary>
    /// <param name="message">Message to display next to the counter.</param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Mark(string message = "")
    {
        Debug.Log(String.Format("{0} {1}", marker, message));
        marker++;
    }


    /// <summary>
    /// Display message to the ConsoleLine.
    /// </summary>
    /// <param name="message">Message to display.</param>
    /// <param name="time">Time in seconds to display the message for. 0s is for one frame.</param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogConsoleLine(string message, float time = 0f)
    {
        if (Main.consoleLine == null) return;

        Messages.Enqueue(new KeyValuePair<string, float>(message, time));
        if (!displayingMessages)
        {
            Main.StartCoroutine(DisplayConsoleLine());
        }
    }


    /// <summary>
    /// Displays all messages to ConsoleLine in the order that they were recieved.
    /// </summary>
    private static IEnumerator DisplayConsoleLine()
    {
        displayingMessages = true;
        while (Messages.Count > 0)
        {
            var message = Messages.Dequeue();
            Main.consoleLine.guiText.text = message.Key;
            yield return new WaitForSeconds(message.Value);
        }

        Main.consoleLine.guiText.text = "";
        displayingMessages = false;
    }

    #endregion
}