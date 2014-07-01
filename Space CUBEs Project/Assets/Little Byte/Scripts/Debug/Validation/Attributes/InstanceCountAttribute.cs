// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.29
// Edited: 2014.06.30

using System;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Validates the number of instances of a certain class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InstanceCountAttribute : ValidationAttribute
    {
        #region Readonly Fields

        /// <summary>How many instances of this class there needs to be.</summary>
        private readonly int count;

        #endregion

        #region Static Fields

        /// <summary>Number of instances found.</summary>
        private static int? found;

        #endregion

        #region Constructors

        /// <param name="count">How many instances of this class there needs to be.</param>
        public InstanceCountAttribute(int count = 1)
        {
            this.count = count;
        }


        /// <summary>
        /// Register to ClearEvent for nulling found.
        /// </summary>
        static InstanceCountAttribute()
        {
            ClearEvent += (sender, args) => found = null;
        }

        #endregion

        #region ValidatingAttribute Overrides

        /// <inheritdoc/>
        public override bool IsValidValue(object value)
        {
            // wrong type
            if (!(value is UnityEngine.Object))
            {
                return Failed("Attribute can only be applied to {0}.", typeof(UnityEngine.Object).Name);
            }

            // not correct number
            if (found == null)
            {
                found = UnityEngine.Object.FindObjectsOfType(value.GetType()).Length;
            }
            if (found.Value != count)
            {
                return Failed("There needs to be {0} {1} class instances. There are currently {2}.", count, value.GetType().Name, found.Value);
            }

            return true;
        }

        #endregion
    }
}