// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.04
// Edited: 2014.10.04

using Annotations;
using UnityEngine;

public class GoogleAnalytics : Singleton<GoogleAnalytics>
{
    #region Private Fields

    [SerializeField, UsedImplicitly]
    private GoogleAnalyticsV3 analytics;

    #endregion

    #region Static Methods

    public static void StartSession()
    {
        //Main.analytics.StartSession();
    }


    public static void StopSession()
    {
        //Main.analytics.StopSession();
    }


    public static void LogEvent(string eventCategory, string eventAction, string eventLabel, long value)
    {
        //Main.analytics.LogEvent(eventCategory, eventAction, eventLabel, value);
    }

    #endregion
}