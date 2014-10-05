// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.05
// Edited: 2014.10.05

using System.Collections;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class TouchInput : MonoBehaviour, IInputController
    {
        #region Private Fields

        private bool barrelRollPressed;
        private ButtonStates[] weaponStates = new ButtonStates[Player.Weaponlimit];

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Awake()
        {
            HUD.Main.barrelRoll.ActivateEvent += OnBarrelRollPressed;
            for (int i = 0; i < Player.Weaponlimit; i++)
            {
                HUD.Main.weaponButtons[i].ActivateEvent += OnWeaponPressed;
            }
        }


        [UsedImplicitly]
        private void LateUpdate()
        {
            for (int i = 0; i < Player.Weaponlimit; i++)
            {
                if (weaponStates[i] == ButtonStates.Pressed)
                {
                    weaponStates[i] = ButtonStates.Held;
                }
                else if (weaponStates[i] == ButtonStates.Released)
                {
                    weaponStates[i] = ButtonStates.None;
                }
            }
        }

        #endregion

        #region IInputController Overrides

        public Vector2 Joystick()
        {
            return HUD.Main.joystick.value;
        }


        public bool BarrelRoll()
        {
            return barrelRollPressed;
        }


        public ButtonStates[] Weapons()
        {
            return weaponStates;
        }

        #endregion

        #region Private Methods

        private IEnumerator BarrelRolling()
        {
            barrelRollPressed = true;
            yield return null;
            barrelRollPressed = false;
        }

        #endregion

        #region Event Handlers

        private void OnBarrelRollPressed(object sender, ActivateButtonArgs args)
        {
            if (!args.isPressed) return;

            StartCoroutine(BarrelRolling());
        }


        private void OnWeaponPressed(object sender, ActivateButtonArgs args)
        {
            int weapon = int.Parse(args.value);

            weaponStates[weapon] = args.isPressed ? ButtonStates.Pressed : ButtonStates.Released;
        }

        #endregion
    }
}


//foreach (Touch touch in Input.touches)
//            {
//                if (touch.position.x > Screen.width / 2f)
//                {
//                    if (touch.deltaPosition.y >= 15f)
//                    {
//                        return true;
//                    }
//                }
//            }