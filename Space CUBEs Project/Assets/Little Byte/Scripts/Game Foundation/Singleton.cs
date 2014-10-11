// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.06.13

using System;
using UnityEngine;

/// <summary>
/// Generic Singleton base class.
/// </summary>
/// <typeparam name="T">Class that wants to be a singleton.</typeparam>
public class Singleton<T> : MonoBase where T : MonoBehaviour
{
    #region Public Fields

    /// <summary>Should a warning be logged if there are multiple occurences of singleton.</summary>
    //[Header("Singleton")]
    [Tooltip("Should a warning be logged if there are multiple occurences of singleton.")]
    public bool logWarning;

    /// <summary>Should this game object not be destroyed on load?</summary>
    [Tooltip("Should this game object not be destroyed on load?")]
    public bool dontDestroy;

    #endregion

    #region Static Fields

    /// <summary>Singleton instance of class.</summary>
    public static T Main { get; protected set; }

    #endregion

    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        if (Main != null)
        {
            if (logWarning)
            {
                Debugger.LogWarning("Multiple instances of " + typeof(T).Name);
            }
            enabled = false;
            Destroy(gameObject);
        }
        else
        {
            Main = GetComponent<T>();
            if (dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    #endregion

    #region Static Methods

#if UNITY_EDITOR
    public T Find()
    {
        var objects = Resources.FindObjectsOfTypeAll<T>();

        if (objects.Length == 0)
        {
            Debugger.LogException(new Exception("No singleton of type " + typeof(T).Name + " found."));
        }
        else if (objects.Length > 1)
        {
            Debugger.LogException(new Exception("More than 1 singleton of type " + typeof(T).Name + " found."));
        }

        return Resources.FindObjectsOfTypeAll<T>()[0];
    } 
#endif

    #endregion
}