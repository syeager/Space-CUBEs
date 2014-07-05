// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.06.13

using UnityEngine;

/// <summary>
/// Generic Singleton base class.
/// </summary>
/// <typeparam name="T">Class that wants to be a singleton.</typeparam>
public class Singleton<T> : MonoBase where T : MonoBehaviour
{
    #region Public Fields

    /// <summary>Should a warning be logged if there are multiple occurences of singleton.</summary>
    public bool logWarning;

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
        }
    }

    #endregion
}