// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.01
// Edited: 2014.06.13

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

/// <summary>
/// Wrapper for Unity's Debug class.
/// </summary>
public class Debugger : Singleton<Debugger>
{
    #region References

    public HUDFPS fps;
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
        true, // Default
        true, // Data
        true, // LevelEvents
        true, // StateMachines
        true, // Construction
        true, // Performance
    };

    /// <summary>Catagorization for logging. Can be added to but need to update logFlags as well.</summary>
    public enum LogTypes
    {
        Default = 0,
        Data = 1,
        LevelEvents = 2,
        StateMachines = 3,
        Construction = 4,
        Performance = 5,
    }

    /// <summary>Should the Editor pause if at a low FPS?</summary>
    public bool fpsWarning = true;

    /// <summary>Lowest FPS allowed before pausing.</summary>
    public int lowFPS = 20;

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

    /// <summary>Path for log files.</summary>
    private static string logPath = "/Files/Debug Logs/Log_";

    /// <summary>Are messages currently being displayed to the ConsoleLine?</summary>
    private static bool displayingMessages;

    /// <summary>Current marker for Debug.Mark.</summary>
    private static int marker;

    #endregion

    #region Readonly Fields

    /// <summary>Messages queued up for the ConsoleLine and their time in seconds to be displayed.</summary>
    private static readonly Queue<KeyValuePair<string, float>> Messages = new Queue<KeyValuePair<string, float>>();

    #endregion

    #region Const Fields

    /// <summary>File extention for log files.</summary>
    private const string FileExt = ".txt";

    /// <summary>Time in seconds to get average FPS.</summary>
    private const float UpdateInterval = 0.5f;

    private const string Yellow = "<color=#ffff00>";
    private const string Red = "<color=#ff0000>";
    private const string EndColor = "</color>";

    #endregion

    #region MonoBehaviour Overrides

#if UNITY_EDITOR
    /// <summary>
    /// Set up log files.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (!enabled) return;
        DontDestroyOnLoad(gameObject);

        if (logSaving == LogSaving.DontSave) return;

        logPath = Application.dataPath + logPath;

        string[] logTypes = Enum.GetNames(typeof(LogTypes));
        for (int i = 0; i < logFlags.Length; i++)
        {
            string path = logPath + logTypes[i] + FileExt;
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
            fps.UpdateFPS(average);

            if (fpsWarning && average < lowFPS)
            {
#if UNITY_EDITOR
                Debug.Break();
#else
                LogError(string.Format("FPS below {0}", lowFPS), null, LogTypes.Performance);
#endif
            }

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
            Destroy(fps.gameObject);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Clear every log file.
    /// </summary>
    public void ClearLogs()
    {
        logPath = Application.dataPath + logPath;

        string[] logTypes = Enum.GetNames(typeof(LogTypes));
        for (int i = 0; i < logFlags.Length; i++)
        {
            File.WriteAllText(logPath + logTypes[i] + FileExt, String.Empty);
        }
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Display a message and pause the Editor.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    public static void Break(object message, Object context = null)
    {
        Debug.Log(message, context);
        Debug.Break();
    }


    /// <summary>
    /// Displays a log message to the console. Doesn't use Singleton.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    /// <param name="args">Arguments for string.Format.</param>
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void Print(string message, params object[] args)
    {
        Debug.Log(string.Format(message, args));
    }


    /// <summary>
    /// Displays a log message to the Console.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    /// <param name="save">Should this message be saved to a log file?</param>
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void Log(object message, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        if (Application.isPlaying)
        {
            if (Main.logFlags[(int)logType])
            {
                Debug.Log((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + message, context);
            }
        }
        else
        {
            Debug.Log(string.Format("[{0}] {1}", DateTime.Now.ToShortTimeString(), message), context);
        }

#if UNITY_EDITOR
        if (Application.isPlaying && save && Main.logSaving != LogSaving.DontSave)
        {
            using (var writer = new StreamWriter(logPath + logType + FileExt, true))
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
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void LogWarning(object message, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        if (Application.isPlaying)
        {
            if (Main.logFlags[(int)logType])
            {
                Debug.LogWarning(Yellow + (Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + message + EndColor, context);
            }
        }
        else
        {
            Debug.LogWarning(Yellow + string.Format("[{0}] {1}", DateTime.Now.ToShortTimeString() + EndColor, message), context);
        }

#if UNITY_EDITOR
        if (Application.isPlaying && save && Main.logSaving != LogSaving.DontSave)
        {
            using (var writer = new StreamWriter(logPath + logType + FileExt, true))
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
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void LogError(object message, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        if (Application.isPlaying)
        {
            Debug.LogError(Red + (Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + message + EndColor, context);
        }
        else
        {
            Debug.LogError(Red + string.Format("[{0}] {1}", DateTime.Now.ToShortTimeString(), message) + EndColor, context);
        }

#if UNITY_EDITOR
        if (Application.isPlaying && save && Main.logSaving != LogSaving.DontSave)
        {
            using (var writer = new StreamWriter(logPath + logType + FileExt, true))
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
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Exception LogException(Exception exception, Object context = null, LogTypes logType = LogTypes.Default, bool save = true)
    {
        Debug.LogException(exception, context);

#if UNITY_EDITOR
        if (Application.isPlaying && save && Main.logSaving != LogSaving.DontSave)
        {
            using (var writer = new StreamWriter(logPath + logType + FileExt, true))
            {
                writer.WriteLine("[{0}] Exception\r\n {1}", Time.realtimeSinceStartup, exception);
            }
        }
#endif

        return exception;
    }


    /// <summary>
    /// Runs through a list and displays its contents to the Console.
    /// </summary>
    /// <param name="list">List to display.</param>
    /// <param name="header">Message to display before the list.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void LogList(IEnumerable list, string header = "", Object context = null, LogTypes logType = LogTypes.Default)
    {
        if (Application.isPlaying)
        {
            if (!Main.logFlags[(int)logType]) return;

            Debug.Log((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + header + '\n', context);
        }
        else
        {
            Debug.Log(string.Format("[{0}] {1}\n", DateTime.Now.ToShortTimeString(), header), context);
        }

        int count = 0;
        foreach (object item in list)
        {
            Debug.Log(count + ": " + item, item is Object ? (Object)item : context);
            count++;
        }

        if (count == 0)
        {
            Debug.Log("Empty", context);
        }
    }


    /// <summary>
    /// Runs through a dictionary and displays its Keys and Values to the Console.
    /// </summary>
    /// <param name="dictionary">Dictionary to display.</param>
    /// <param name="header">Message to display before the list.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void LogDict<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string header = "", Object context = null, LogTypes logType = LogTypes.Default)
    {
        if (Application.isPlaying)
        {
            if (!Main.logFlags[(int)logType]) return;

            Debug.Log((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + header + '\n', context);
        }
        else
        {
            Debug.Log(string.Format("[{0}] {1}\n", DateTime.Now.ToShortTimeString(), header), context);
        }

        int count = 0;
        foreach (var item in dictionary)
        {
            Object itemContext;
            if (item.Key is Object)
            {
                itemContext = item.Key as Object;
            }
            else if (item.Value is Object)
            {
                itemContext = item.Value as Object;
            }
            else
            {
                itemContext = context;
            }
            Debug.Log(count + ": " + item.Key + ", " + item.Value, itemContext);
            count++;
        }

        if (count == 0)
        {
            Debug.Log("Empty", context);
        }
    }


    /// <summary>
    /// Display an instance's fields to the Console.
    /// </summary>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    /// <param name="header">Message to display before the list.</param>
    /// <param name="includePrivate">Should private members be shown as well?</param>
    /// <param name="logType">Will only display this message if this LogType is checked as active.</param>
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void LogFields(object context, string header, bool includePrivate = false, LogTypes logType = LogTypes.Default)
    {
        var unityContext = context as Object;

        if (Application.isPlaying)
        {
            if (!Main.logFlags[(int)logType]) return;

            Debug.Log((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + header + '\n', unityContext);
        }
        else
        {
            Debug.Log(string.Format("[{0}] {1}\n", DateTime.Now.ToShortTimeString(), header), unityContext);
        }

        Type type = context.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        if (includePrivate) flags |= BindingFlags.NonPublic;
        FieldInfo[] info = type.GetFields(flags);

        foreach (FieldInfo i in info)
        {
            object o = i.GetValue(context);
            Debug.Log(i.Name + ": " + o, o is Object ? o as Object : unityContext);
        }
    }


    /// <summary>
    /// Updates an internal counter and displays current value to the Console.
    /// </summary>
    /// <param name="message">Message to display next to the counter.</param>
    /// <param name="context">Unity Object to highlight in the Hierarchy.</param>
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void Mark(string message = "", Object context = null)
    {
        Debug.Log((Main.showTime ? "[" + Time.realtimeSinceStartup + "] " : "") + String.Format("{0} {1}", marker, message), context);
        marker++;
    }


    /// <summary>
    /// Checks if context is null.
    /// </summary>
    /// <param name="context">object to check.</param>
    /// <param name="message">Optional message to log.</param>
    /// <returns>True, if null.</returns>
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static bool NullCheck(object context, string message = "")
    {
        if (context == null)
        {
            Debug.Log("Null: " + message);
            return true;
        }
        else
        {
            Debug.Log("Not Null: " + (message == "" ? context.GetType().Name : message));
            return false;
        }
    }


    /// <summary>
    /// Display message to the ConsoleLine.
    /// </summary>
    /// <param name="message">Message to display.</param>
    /// <param name="time">Time in seconds to display the message for. 0s is for one frame.</param>
    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
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
    [DebuggerHidden]
    [DebuggerStepThrough]
    private static IEnumerator DisplayConsoleLine()
    {
        displayingMessages = true;
        while (Messages.Count > 0)
        {
            KeyValuePair<string, float> message = Messages.Dequeue();
            Main.consoleLine.guiText.text = message.Key;
            yield return new WaitForSeconds(message.Value);
        }

        Main.consoleLine.guiText.text = "";
        displayingMessages = false;
    }

    #endregion
}