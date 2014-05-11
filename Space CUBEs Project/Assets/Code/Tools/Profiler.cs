// Steve Yeager
// 4.2.2014

using System;
using System.Diagnostics;

namespace LittleByte.Debug
{
    /// <summary>
    /// Uses a Stopwatch to time code execution.
    /// </summary>
    public class Profiler : IDisposable
    {
        #region Private Fields

        private readonly string message;
        private readonly Stopwatch stopwatch;

        #endregion


        #region Constructors

        public Profiler(string message)
        {
            this.message = message;
            stopwatch = Stopwatch.StartNew();
        }

        #endregion

        #region IDisposable Overrides

        public void Dispose()
        {
            stopwatch.Stop();
            Debugger.Log(string.Format("Profiled {0}: {1:0.00}ms", message, stopwatch.ElapsedMilliseconds), null, Debugger.LogTypes.Performance);
        }

        #endregion
    }
}