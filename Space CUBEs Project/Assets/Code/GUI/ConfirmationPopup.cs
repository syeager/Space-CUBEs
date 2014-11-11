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

        private bool destroy;

        #endregion

        #region MonoBehaviour Overrides

        private void Awake()
        {
            transform.parent = UIRoot.list[0].transform;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        #endregion

        #region Public Methods

        public void Initialize(Action<bool> onComplete, string message = "", string confirm = "Accept", string deny = "Cancel", bool destroy = false)
        {
            gameObject.SetActive(true);
            this.onComplete = onComplete;
            this.destroy = destroy;

            if (!string.IsNullOrEmpty(message))
            {
                messageLabel.text = message;
            }
            confirmLabel.text = confirm;
            denyLabel.text = deny;
        }

        public void Deny()
        {
            onComplete(false);
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Confirm()
        {
            onComplete(true);
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}