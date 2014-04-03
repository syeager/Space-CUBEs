// Steve Yeager
// 4.2.2014

using UnityEngine;
using System;
using System.Diagnostics;

/// <summary>
/// Uses a Stopwatch to time code execution.
/// </summary>
public class SpeedTimer : IDisposable
{
    #region Private Fields

    private readonly string message;
    private readonly Stopwatch stopwatch;

    #endregion


    #region Constructors

    public SpeedTimer(string message)
    {
        this.message = message;
        stopwatch = Stopwatch.StartNew();
    }

    #endregion

    #region IDisposable Overrides

    public void Dispose()
    {
        stopwatch.Stop();
        UnityEngine.Debug.Log(string.Format("Profiled {0}: {1:0.00}ms", message, stopwatch.ElapsedMilliseconds));
    }

    #endregion
}