// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.05
// Edited: 2014.10.05

using System.Collections;
using UnityEngine;

namespace SpaceCUBEs
{
    public class DeviceInput : MonoBehaviour, IInputController
    {
        #region Private Fields

        private readonly ButtonStates[] weaponStates = new ButtonStates[Player.Weaponlimit];
        private bool canBarrelRollTrigger = true;

        #endregion

        #region Const Fields

        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";
        private const string WeaponInput = "Weapon";
        private const string BarrelRollInput = "BarrelRoll";

        private const float BarrelRollTrigger = 0.5f;

        #endregion

        #region IInputController Overrides

        public Vector2 Joystick()
        {
            return new Vector2(Input.GetAxis(HorizontalInput), Input.GetAxis(VerticalInput)).normalized;
        }


        public bool BarrelRoll()
        {
            if (Input.GetButtonDown(BarrelRollInput))
            {
                return true;
            }
            else if (canBarrelRollTrigger && Input.GetAxis(BarrelRollInput) >= BarrelRollTrigger)
            {
                StartCoroutine(ResetBarrelRollTrigger());
                return true;
            }

            return false;
        }


        public ButtonStates[] Weapons()
        {
            for (int i = 0; i < Player.Weaponlimit; i++)
            {
                if (Input.GetButtonDown(WeaponInput + i))
                {
                    weaponStates[i] = ButtonStates.Pressed;
                }
                else if (Input.GetButtonUp(WeaponInput + i))
                {
                    weaponStates[i] = ButtonStates.Released;
                }
                else
                {
                    // don't need held
                    weaponStates[i] = ButtonStates.None;
                }
            }

            return weaponStates;
        }

        #endregion

        #region Private Methods

        private IEnumerator ResetBarrelRollTrigger()
        {
            canBarrelRollTrigger = false;

            while (Input.GetAxis(BarrelRollInput) >= BarrelRollTrigger)
            {
                yield return null;
            }

            canBarrelRollTrigger = true;
        }

        #endregion
    }
}