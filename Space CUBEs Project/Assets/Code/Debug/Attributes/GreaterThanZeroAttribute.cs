// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

using System;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates a specified class Field/Property to be greater than 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GreaterThanZeroAttribute : ValidatingAttribute
    {
        /// <inheritdoc />
        public override bool IsValidValue(object value)
        {
            Type valueType = value.GetType();
            if (valueType == typeof(int))
            {
                return ((int)(value)) > 0;
            }
            if (valueType == typeof(float))
            {
                return ((float)(value)) > 0;
            }
            if (valueType == typeof(double))
            {
                return ((double)(value)) > 0;
            }

            //unknown type.
            throw Debugger.LogException(new TypeNotSupportedException(value));
        }


        /// <inheritdoc />
        public override string GetDescOfValid()
        {
            return "Valid values are GREATER THAN zero.  Includes checks for array length greater than zero.";
        }
    }
}