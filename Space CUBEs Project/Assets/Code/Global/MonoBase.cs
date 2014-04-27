// Steve Yeager
// 12.4.2013

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

// TODO: Look into this class.
public class MonoBase : MonoBehaviour
{
    #region Public Fields

    public bool log = false;
    public float timeScale = 1f;

    #endregion

    #region Properties

    public float deltaTime { get { return Time.deltaTime * timeScale; } }

    #endregion

    #region Log Methods

    [Conditional("DEBUG")]
    protected void Log(object message, bool save = true, Debugger.LogTypes logType = Debugger.LogTypes.Default)
    {
        if (log)
        {
            Debugger.Log(message, gameObject, logType, save);
        }
    }


    [Conditional("DEBUG")]
    protected void LogWarning(object message, bool save = true, Debugger.LogTypes logType = Debugger.LogTypes.Default)
    {
        if (log)
        {
            Debugger.LogWarning(message, gameObject, logType, save);
        }
    }


    [Conditional("DEBUG")]
    protected void LogError(object message, bool save = true, Debugger.LogTypes logType = Debugger.LogTypes.Default)
    {
        if (log)
        {
            Debugger.LogError(message, gameObject, logType, save);
        }
    }

    #endregion

    #region Invoke Methods

    /// <summary>
    /// Call delayed Action.
    /// </summary>
    /// <param name="action">Action to call.</param>
    /// <param name="time">Time in GameTimeSeconds.</param>
    public void InvokeAction(Action action, float time)
    {
        StartCoroutine(InvokedAction(action, time));
    }


    private IEnumerator InvokedAction(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    #endregion
}