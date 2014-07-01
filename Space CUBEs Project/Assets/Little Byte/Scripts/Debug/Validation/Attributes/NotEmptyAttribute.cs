// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.30

using System;
using System.Collections;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates that a string is not null or empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotEmptyAttribute : ValidationAttribute
    {
        #region ValidatingAttribute Overrides

        /// <inheritdoc/>
        public override bool IsValidValue(object value)
        {
            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value) || Failed("String cannot be empty.");
            }
            if (value is ICollection)
            {
                return ((ICollection)value).Count == 0 || Failed("Collection cannot be empty.");
            }

            return Failed("Type \'{0}\' is not supported.", value.GetType().Name);
        }

        #endregion
    }
}