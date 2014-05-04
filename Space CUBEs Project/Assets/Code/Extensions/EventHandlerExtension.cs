// Steve Yeager
// 4.6.2014
// Found online.

using System;

/// <summary>
/// Extension methods for EventHandlers.
/// </summary>
public static class EventHandlerExtension
{
    #region Extension Methods

    /// <summary>
    /// Send event if it is not empty.
    /// </summary>
    public static void Fire<T>(this EventHandler<T> self, object sender, T args) where T : EventArgs
    {
        if (self != null) self(sender, args);
    }


    /// <summary>
    /// Send event if it is not empty.
    /// </summary>
    public static void Fire(this EventHandler self, object sender, EventArgs args)
    {
        if (self != null) self(sender, args);
    }
    
    #endregion
}