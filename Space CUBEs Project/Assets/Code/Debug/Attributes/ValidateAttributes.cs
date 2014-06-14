// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

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
                foreach (ValidatingAttribute validatingAttrib in fieldInfo.GetCustomAttributes(true).Where(validatingAttrib => validatingAttrib is ValidatingAttribute))
                {
                    if (log)
                    {
                        Debugger.Log("Testing " + validatingAttrib.GetType().Name + " on: Class \'" + objInstance.GetType() +
                                     "\' with Instance \'" + objInstance + "\' with a Field \'" + fieldInfo.Name +
                                     "\' which has value of \'" + fieldInfo.GetValue(objInstance) + "\'. ");
                    }

                    if (!validatingAttrib.IsValidValue(fieldInfo.GetValue(objInstance)))
                    {
                        exceptions.Add(new Exception(validatingAttrib.GetFieldErrorMessage(objInstance, fieldInfo)));
                    }
                }
            }

            // properties
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                foreach (ValidatingAttribute validatingAttrib in propertyInfo.GetCustomAttributes(true).Where(validatingAttrib => validatingAttrib is ValidatingAttribute))
                {
                    if (log)
                    {
                        Debugger.Log("Testing " + validatingAttrib.GetType().Name + " on: Class \'" + objInstance.GetType() +
                                     "\' with Instance \'" + objInstance + "\' with a Field \'" + propertyInfo.Name +
                                     "\' which has value of \'" + propertyInfo.GetValue(objInstance, null) + "\'. ");
                    }

                    if (!validatingAttrib.IsValidValue(propertyInfo.GetValue(objInstance, null)))
                    {
                        exceptions.Add(new Exception(validatingAttrib.GetFieldErrorMessage(objInstance, propertyInfo)));
                    }
                }
            }

            return exceptions;
        }
    }
}