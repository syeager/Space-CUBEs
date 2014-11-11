// Little Byte Games

using System;

namespace SpaceCUBEs
{
    public class InputPopup : ConfirmationPopup
    {
        #region Public Fields

        public UIInput input;

        #endregion

        #region ConfirmationPopup Overrides

        public void Initialize(Action<bool> onComplete, string input, string message = "", string confirm = "Accept", bool destroy = false, string deny = "Cancel")
        {
            this.input.value = input;
            Initialize(onComplete, message, confirm, destroy, deny);
        }

        #endregion
    }
}