// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.30

using System;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// A type of Attribute that will be used to Validate Fields and Properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ValidationAttribute : Attribute
    {
        #region Properties

        /// <summary>Error message from the last time validation was run.</summary>
        public string ErrorMessage { get; protected set; }

        #endregion

        #region Events

        /// <summary>Event that is fired after validation is run. Used to reset static members.</summary>
        public static EventHandler ClearEvent;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Set error message and return false.
        /// </summary>
        /// <param name="message">Error message for string.Format.</param>
        /// <param name="args">Arguments for string.Format.</param>
        /// <returns>False.</returns>
        protected bool Failed(string message, params object[] args)
        {
            ErrorMessage = string.Format(message, args);
            return false;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Test if this attribute is valid or not.
        /// </summary>
        /// <param name="value">The current value of the field being tested.</param>
        /// <returns>True, if valid.</returns>
        public abstract bool IsValidValue(object value);

        #endregion

        #region Statc Methods

        /// <summary>
        /// Fire off clear event.
        /// </summary>
        public static void Clear()
        {
            ClearEvent.Fire();
        }

        #endregion
    }
}