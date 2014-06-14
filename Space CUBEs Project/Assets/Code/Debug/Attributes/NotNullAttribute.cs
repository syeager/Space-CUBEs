// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

using System;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    ///  Validates a specified class Field/Property to be not null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : ValidatingAttribute
    {
        /// <inheritdoc />
        public override bool IsValidValue(object value)
        {
            return value != null;
        }


        /// <inheritdoc />
        public override string GetDescOfValid()
        {
            return "Valid values are NOT null.";
        }
    }
}