// Little Byte Games

using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class InputManager : Singleton<InputManager>
    {
        #region Private Fields

        public enum Inputs
        {
            Touch,
            Gamepad,
            Keyboard,
        }

        public static Inputs ActiveInput { get; private set; }

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Update()
        {
            switch (ActiveInput)
            {
                case Inputs.Gamepad:
                    if (Input.GetJoystickNames().Length == 0)
                    {
                        if (Input.multiTouchEnabled)
                        {
                            ActiveInput = Inputs.Touch;
                        }
                        else
                        {
                            Screen.showCursor = true;
                            ActiveInput = Inputs.Keyboard;
                        }
                    }
                    break;

                case Inputs.Keyboard:
                    if (Input.GetJoystickNames().Length >= 1)
                    {
                        Screen.showCursor = false;
                        ActiveInput = Inputs.Gamepad;
                    }
                    break;

                case Inputs.Touch:
                    if (Input.GetJoystickNames().Length >= 1)
                    {
                        ActiveInput = Inputs.Gamepad;
                    }
                    break;
            }
        }

        #endregion
    }
}