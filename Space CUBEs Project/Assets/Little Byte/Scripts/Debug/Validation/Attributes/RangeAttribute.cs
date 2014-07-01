// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.14
// Edited: 2014.06.30

using System;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates a specified class Field/Property to be between a set range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RangeAttribute : ValidationAttribute
    {
        #region Readonly Fields

        /// <summary>Low value to be above.</summary>
        private readonly object min;

        /// <summary>High value to be below.</summary>
        private readonly object max;

        #endregion

        #region Constructors

        /// <param name="min">Low value to be above.</param>
        /// <param name="max">High value to be below.</param>
        public RangeAttribute(object min, object max)
        {
            this.min = min;
            this.max = max;
        }

        #endregion

        #region ValidationAttribute Overrides

        /// <inheritdoc />
        public override bool IsValidValue(object value)
        {
            Type valueType = value.GetType();

            // mismatched types
            if (min.GetType() != valueType || max.GetType() != valueType)
            {
                return Failed("Min [{0}] and/or Max [{1}] is not of the same type as the object [{2}] its attributed to.", min.GetType(), max.GetType(), valueType);
            }

            if (valueType == typeof(int))
            {
                bool valid = (int)(value) >= (int)min && (int)(value) <= (int)max;
                return valid || Failed("Value \'{0}\' is not within the range [{1}, {2}].", value, min, max);
            }
            if (valueType == typeof(uint))
            {
                bool valid = (uint)(value) >= (uint)min && (uint)(value) <= (uint)max;
                return valid || Failed("Value \'{0}\' is not within the range [{1}, {2}].", value, min, max);
            }
            if (valueType == typeof(float))
            {
                bool valid = (float)(value) >= (float)min && (float)(value) <= (float)max;
                return valid || Failed("Value \'{0}\' is not within the range [{1}, {2}].", value, min, max);
            }
            if (valueType == typeof(double))
            {
                bool valid = (double)(value) >= (double)min && (double)(value) <= (double)max;
                return valid || Failed("Value \'{0}\' is not within the range [{1}, {2}].", value, min, max);
            }

            return Failed("Type \'{0}\' is not supported.", valueType.Name);
        }

        #endregion
    }
}