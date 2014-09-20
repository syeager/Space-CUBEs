// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.16
// Edited: 2014.09.16

using Annotations;
using System.Collections;
using UnityEngine;

namespace SpaceCUBEs
{
    public class GarageMenuButton : ScrollviewButton
    {
        #region Public Fields

        public GarageManager.Menus menu;

        [SerializeField, UsedImplicitly]
        private float speed;

        [SerializeField, UsedImplicitly]
        private float closeY;

        [SerializeField, UsedImplicitly]
        private float openY;

        #endregion

        #region Private Fields

        private bool canPress; 

        #endregion

        #region ScrollviewButton Overrides

        protected override void OnPress(bool isPressed)
        {
            if (!canPress) return;

            base.OnPress(isPressed);
        }

        #endregion

        #region Public Methods

        public void Open(bool open)
        {
            canPress = false;
            if (open)
            {
                gameObject.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(Opening());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(Closing());
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator Opening()
        {
           while (transform.localPosition.y < openY)
            {
                transform.localPosition += Vector3.up * speed * Time.deltaTime;
                yield return null;
            }
            transform.localPosition = new Vector3(0f, openY, 0f);
            canPress = true;
        }


        private IEnumerator Closing()
        {
            while (transform.localPosition.y > closeY)
            {
                transform.localPosition -= Vector3.up * speed * Time.deltaTime;
                yield return null;
            }
            transform.localPosition = new Vector3(0f, closeY, 0f);
            gameObject.SetActive(false);
        }

        #endregion
    }
}