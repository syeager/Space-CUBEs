// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.20
// Edited: 2014.10.20

using Annotations;
using UnityEngine;

public class Gamepad : Singleton<Gamepad>
{
    #region Private Fields

    private bool gamepad;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Update()
    {
        if (gamepad)
        {
            if (Input.GetJoystickNames().Length == 0)
            {
                gamepad = false;
                if (!Input.multiTouchEnabled)
                {
                    Screen.showCursor = true;
                }
            }
        }
        else
        {
            if (Input.GetJoystickNames().Length > 0)
            {
                gamepad = true;
                if (!Input.multiTouchEnabled)
                {
                    Screen.showCursor = false;
                }
            }
        }
    }

    #endregion
}