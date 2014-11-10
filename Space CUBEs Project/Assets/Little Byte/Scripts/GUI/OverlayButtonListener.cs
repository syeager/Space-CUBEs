// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.03
// Edited: 2014.10.19

using System.Linq;
using Annotations;
using UnityEngine;

public class OverlayButtonListener : MonoBehaviour
{
    #region Private Fields

    [SerializeField, UsedImplicitly]
    private string[] groups;

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
        if (groups.Length > 0 && groups.Contains(overlayEventArgs.group)) return;

        var button = GetComponent<UIButton>();

        if (button != null)
        {
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
        else
        {
            collider.enabled = !overlayEventArgs.activated;
        }
    }

    #endregion
}