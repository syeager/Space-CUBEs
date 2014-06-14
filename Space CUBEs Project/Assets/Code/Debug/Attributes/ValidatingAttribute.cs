// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

using System;
using System.Reflection;


namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// A type of Attribute that will be used to Validate Fields and Properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ValidatingAttribute : Attribute
    {
        /// <summary>
        /// Test if this attribute is valid or not.
        /// </summary>
        /// <param name="value">The current value of the field being tested.</param>
        /// <returns>True, if valid.</returns>
        public abstract bool IsValidValue(object value);


        /// <summary>
        /// Get a short, human readable, string that describes what this class considers to be valid or invalid.
        /// </summary>
        /// <returns>
        /// A human readable string that describes valid and/or invalid values.
        /// </returns>
        public abstract string GetDescOfValid();


        /// <summary>
        /// Build the error message for when this validation fails.
        /// </summary>
        /// <param name='objInstance'>Instance of an object being checked when IsValidValue() faild.</param>
        /// <param name='fieldInfo'>The field within the object that just failed the IsValidValue() check.</param>
        /// <returns>A human readable string.</returns>
        public virtual string GetFieldErrorMessage(object objInstance, FieldInfo fieldInfo)
        {
            return GetType().Name + " Failed: Class \'" + objInstance.GetType() + "\' has Instance \'" + objInstance +
                   "\' with a Field \'" + fieldInfo.Name + "\' which has an INVALID VALUE of \'" +
                   fieldInfo.GetValue(objInstance) + "\'. " + GetDescOfValid();
        }


        /// <summary>
        /// Build the error message for when this validation fails.
        /// </summary>
        /// <param name='objInstance'>Instance of an object being checked when IsValidValue() faild.</param>
        /// <param name='propertyInfo'>The property within the object that just failed the IsValidValue() check.</param>
        /// <returns>A human readable string.</returns>
        public virtual string GetFieldErrorMessage(object objInstance, PropertyInfo propertyInfo)
        {
            return GetType().Name + " Failed: Class \'" + objInstance.GetType() + "\' has Instance \'" + objInstance +
                   "\' with a Property \'" + propertyInfo.Name + "\' which has an INVALID VALUE of \'" +
                   propertyInfo.GetValue(objInstance, null) + "\'. " + GetDescOfValid();
        }
    }
}