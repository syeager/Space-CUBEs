// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.14
// Edited: 2014.06.14

using System;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates a specified class Field/Property to be between a set range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RangeAttribute : ValidatingAttribute
    {
        private readonly object min;
        private readonly object max;


        public RangeAttribute(object min, object max)
        {
            this.min = min;
            this.max = max;
        }


        /// <inheritdoc />
        public override bool IsValidValue(object value)
        {
            Type valueType = value.GetType();

            // mismatched types
            if (min.GetType() != valueType || max.GetType() != valueType)
            {
                Debugger.LogException(new ArgumentException(string.Format("Min [{0}] and/or Max [{1}] is not of the same type as the object [{2}] its attributed to.", min.GetType(), max.GetType(), valueType)));
                return false;
            }

            if (valueType == typeof(int))
            {
                return ((int)(value)) >= 0;
            }
            if (valueType == typeof(uint))
            {
                return true;
            }
            if (valueType == typeof(float))
            {
                return ((float)(value)) >= 0;
            }
            if (valueType == typeof(double))
            {
                return ((double)(value)) >= 0;
            }

            //unknown type.
            Debugger.LogException(new TypeNotSupportedException(value));
            return false;
        }


        /// <inheritdoc />
        public override string GetDescOfValid()
        {
            return string.Format("Valid values are within the range [{0}, {1}].", min, max);
        }
    }
}