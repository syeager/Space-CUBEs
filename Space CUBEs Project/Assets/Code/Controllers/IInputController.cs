// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.05
// Edited: 2014.10.05

using UnityEngine;

namespace SpaceCUBEs
{
    public interface IInputController
    {
        Vector2 Joystick();
        bool BarrelRoll();
        ButtonStates[] Weapons();
    }
}