// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.30

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LittleByte.Debug.Attributes
{
    /// <summary>
    /// Static way to run through all the fields in a class and run all the validation attributes that may be present.
    /// </summary>
    public static class ValidateAttributes
    {
        #region Public Methods

        /// <summary>
        /// Checks all attributes on an object.
        /// </summary>
        /// <param name="objInstance">Instance of an object to check.</param>
        /// <param name="log">Should the results be printed to the console?</param>
        /// <returns>All exceptions related to this object.</returns>
        public static List<Exception> Validate(object objInstance, bool log = false)
        {
            List<Exception> exceptions = new List<Exception>();
            Type type = objInstance.GetType();

            // fields
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                foreach (ValidationAttribute attribute in fieldInfo.GetCustomAttributes(true).Where(validatingAttrib => validatingAttrib is ValidationAttribute))
                {
                    if (log)
                    {
                        Debugger.Log("Testing " + attribute.GetType().Name + " on: Class \'" + objInstance.GetType() +
                                     "\' with Instance \'" + objInstance + "\' with a Field \'" + fieldInfo.Name +
                                     "\' which has value of \'" + fieldInfo.GetValue(objInstance) + "\'. ");
                    }

                    if (!attribute.IsValidValue(fieldInfo.GetValue(objInstance)))
                    {
                        exceptions.Add(new Exception(fieldInfo.Name + ": " + attribute.ErrorMessage));
                    }
                }
            }

            // class
            foreach (ValidationAttribute attribute in type.GetCustomAttributes(true).Where(v => v is ValidationAttribute))
            {
                if (log)
                {
                    Debugger.Log("Testing " + attribute.GetType().Name + " on: Class \'" + objInstance.GetType() + "\'. ");
                }

                if (!attribute.IsValidValue(objInstance))
                {
                    exceptions.Add(new Exception(attribute.ErrorMessage));
                }
            }

            return exceptions;
        }

        #endregion
    }
}