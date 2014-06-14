// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

using System;
using System.Collections;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates that a string is not null or empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotEmptyAttribute : ValidatingAttribute
    {
        /// <inheritdoc/>
        public override bool IsValidValue(object value)
        {
            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value);
            }
            if (value is ICollection)
            {
                return ((ICollection)value).Count > 0;
            }

            throw Debugger.LogException(new TypeNotSupportedException(value));
        }


        /// <inheritdoc/>
        public override string GetDescOfValid()
        {
            return "String cannot be null";
        }
    }
}