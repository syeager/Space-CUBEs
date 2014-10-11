// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.09.01

using System;

namespace LittleByte.Extensions
{
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
            if (self != null)
            {
                self(sender, args);
            }
        }


        /// <summary>
        /// Send event if it is not empty.
        /// </summary>
        public static void Fire(this EventHandler self, object sender = null, EventArgs args = null)
        {
            if (self != null) self(sender, args ?? EventArgs.Empty);
        }

        #endregion
    } 
}