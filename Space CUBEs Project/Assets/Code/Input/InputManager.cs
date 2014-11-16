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

        [SerializeField, UsedImplicitly]
        private Inputs activeInput;

        public static Inputs ActiveInput
        {
            get { return Main.activeInput; }
        }

        #endregion

        #region MonoBehaviour Overrides

#if !UNITY_EDITOR
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
                            activeInput = Inputs.Touch;
                        }
                        else
                        {
                            Screen.showCursor = true;
                            activeInput = Inputs.Keyboard;
                        }
                    }
                    break;

                case Inputs.Keyboard:
                    if (Input.GetJoystickNames().Length >= 1)
                    {
                        Screen.showCursor = false;
                        activeInput = Inputs.Gamepad;
                    }
                    break;

                case Inputs.Touch:
                    if (Input.GetJoystickNames().Length >= 1)
                    {
                        activeInput = Inputs.Gamepad;
                    }
                    break;
            }
        }
#endif

        #endregion
    }
}