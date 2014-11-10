// Little Byte Games

using System;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class ConfirmationPopup : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private UILabel messageLabel;

        [SerializeField, UsedImplicitly]
        private UILabel denyLabel;

        [SerializeField, UsedImplicitly]
        private UILabel confirmLabel;

        private Action<bool> onComplete;

        #endregion

        #region Public Methods

        public void Initialize(Action<bool> onComplete, string message = "", string deny = "Cancel", string confirm = "Accept")
        {
            gameObject.SetActive(true);
            this.onComplete = onComplete;

            if (!string.IsNullOrEmpty(message))
            {
                messageLabel.text = message;
            }
            denyLabel.text = deny;
            confirmLabel.text = confirm;
        }

        public void Deny()
        {
            onComplete(false);
            gameObject.SetActive(false);
        }

        public void Confirm()
        {
            onComplete(true);
            gameObject.SetActive(false);
        }

        #endregion
    }
}