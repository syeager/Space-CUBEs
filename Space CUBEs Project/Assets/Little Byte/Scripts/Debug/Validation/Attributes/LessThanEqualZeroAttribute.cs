// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

using System;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates a specified class Field/Property to be less or equal to 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LessThanEqualZeroAttribute : ValidationAttribute
    {
        #region ValidationAttribute Overrides

        /// <inheritdoc />
        public override bool IsValidValue(object value)
        {
            if (value is int)
            {
                bool valid = (int)(value) <= 0;
                return valid || Failed("Value \'{0}\' is not > 0.", value);
            }
            if (value is float)
            {
                bool valid = (float)(value) <= 0;
                return valid || Failed("Value \'{0}\' is not > 0.", value);
            }
            if (value is double)
            {
                bool valid = (double)(value) <= 0;
                return valid || Failed("Value \'{0}\' is not > 0.", value);
            }

            return Failed("Type \'{0}\' is not supported.", value.GetType().Name);
        }

        #endregion
    }
}