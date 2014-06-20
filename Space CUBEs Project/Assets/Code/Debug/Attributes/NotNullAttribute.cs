// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.19

using System;
using UnityEngine;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    ///  Validates a specified class Field/Property to be not null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : ValidatingAttribute
    {
        /// <summary>Allowed to be bull in Edit Mode?</summary>
        private readonly bool allowedInEditMode;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="allowedInEditMode">Allowed to be bull in Edit Mode?</param>
        public NotNullAttribute(bool allowedInEditMode = false)
        {
            this.allowedInEditMode = allowedInEditMode;
        }


        /// <inheritdoc />
        public override bool IsValidValue(object value)
        {
            if (allowedInEditMode && !Application.isPlaying) return true;
            return value != null;
        }


        /// <inheritdoc />
        public override string GetDescOfValid()
        {
            return "Valid values are NOT null.";
        }
    }
}