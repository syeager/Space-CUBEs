// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

using System;
using UnityEngine;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates a specified class Field/Property to be not equal to 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotZeroAttribute : ValidatingAttribute
    {
        /// <inheritdoc/> 
        public override bool IsValidValue(object value)
        {
            Type valueType = value.GetType();
            if (valueType == typeof(int))
            {
                return ((int)(value)) != 0;
            }
            if (valueType == typeof(uint))
            {
                return ((uint)(value)) != 0;
            }
            if (valueType == typeof(float))
            {
                return ((float)(value)) != 0f;
            }
            if (valueType == typeof(double))
            {
                return (double)(value) != 0;
            }
            if (valueType == typeof(Vector3))
            {
                return (Vector3)value != Vector3.zero;
            }

            //unknown type.
            throw Debugger.LogException(new TypeNotSupportedException(value));
        }


        /// <inheritdoc/> 
        public override string GetDescOfValid()
        {
            return
                "Valid values are NOT equal to zero. Includes checks for vectors with all zero elements and arrays with length zero.";
        }
    }
}