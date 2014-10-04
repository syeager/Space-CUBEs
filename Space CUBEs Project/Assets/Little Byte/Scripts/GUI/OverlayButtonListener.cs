﻿// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.03
// Edited: 2014.10.03

using Annotations;
using UnityEngine;

public class OverlayButtonListener : MonoBehaviour
{
    #region Private Fields

    private bool cachedEnable;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        OverlayEventArgs.OverlayEvent += OnOverlay;
    }


    [UsedImplicitly]
    private void OnDestroy()
    {
        OverlayEventArgs.OverlayEvent -= OnOverlay;
    }

    #endregion

    #region Event Handlers

    private void OnOverlay(object sender, OverlayEventArgs overlayEventArgs)
    {
        var button = GetComponent<UIButton>();

        if (overlayEventArgs.activated)
        {
            cachedEnable = button.isEnabled;
            button.isEnabled = false;
        }
        else
        {
            button.isEnabled = cachedEnable;
        }
    }

    #endregion
}