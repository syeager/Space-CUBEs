using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

[RequireComponent(typeof(Collider2D))]
public class TouchButton : MonoBehaviour
{
    #region Public Fields

    public enum States
    {
        Normal,
        Active,
        Disabled
    }
    public States state;
    public enum Triggers
    {
        Pressed,
        Released,
        Held
    }
    public Triggers trigger;

    public string value;

    #endregion

    #region Events

    public EventHandler TriggeredEvent;

    #endregion


    [Conditional("UNITY_STANDALONE")]
    private void Awake()
    {
        Destroy(this);
    }
}