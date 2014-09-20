// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.19
// Edited: 2014.09.19

using System.Collections;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class NavPad : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private float speed;

        [SerializeField, UsedImplicitly]
        private float closedPosition;

        [SerializeField, UsedImplicitly]
        private Transform rotateZButtons;

        [SerializeField, UsedImplicitly]
        private float rotateZPosition;

        [SerializeField, UsedImplicitly]
        private Transform rotateYButtons;

        [SerializeField, UsedImplicitly]
        private float rotateYPosition;

        [SerializeField, UsedImplicitly]
        private Transform navButtons;

        [SerializeField, UsedImplicitly]
        private float navPosition;

        [SerializeField, UsedImplicitly]
        private Transform layerButtons;

        [SerializeField, UsedImplicitly]
        private float layerPosition;

        [SerializeField, UsedImplicitly]
        private GameObject lockButton;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Start()
        {
            GarageManager.Main.MenuChangedEvent += OnMenuChanged;
        }

        #endregion

        #region Public Methods

        public void MoveDown()
        {
            GarageManager.Main.MoveCursor(Vector3.back);
        }


        public void MoveUp()
        {
            GarageManager.Main.MoveCursor(Vector3.forward);
        }


        public void MoveLeft()
        {
            GarageManager.Main.MoveCursor(Vector3.left);
        }


        public void MoveRight()
        {
            GarageManager.Main.MoveCursor(Vector3.right);
        }


        public void MoveBack()
        {
            GarageManager.Main.MoveCursor(Vector3.down);
        }


        public void MoveForward()
        {
            GarageManager.Main.MoveCursor(Vector3.up);
        }


        public void RotateYLeft()
        {
            GarageManager.Main.RotateCursor(Vector3.down);
        }


        public void RotateYRight()
        {
            GarageManager.Main.RotateCursor(Vector3.up);
        }


        public void RotateZLeft()
        {
            GarageManager.Main.RotateCursor(Vector3.forward);
        }


        public void RotateZRight()
        {
            GarageManager.Main.RotateCursor(Vector3.back);
        }

        #endregion

        #region Private Methods

        private IEnumerator SwitchingMenu(GarageManager.Menus currentMenu)
        {
            switch (currentMenu)
            {
                case GarageManager.Menus.Edit:
                    StartCoroutine(Full());
                    break;
                case GarageManager.Menus.Paint:
                case GarageManager.Menus.Abilities:
                    StartCoroutine(Half());
                    break;
                case GarageManager.Menus.View:
                    StartCoroutine(Closed());
                    break;
            }

            yield return null;
        }


        private IEnumerator Full()
        {
            // layers and nav buttons

            // rotate z

            // rotate y

            yield return null;

            lockButton.SetActive(true);
            layerButtons.gameObject.SetActive(true);
            navButtons.gameObject.SetActive(true);
            rotateZButtons.gameObject.SetActive(true);
            rotateYButtons.gameObject.SetActive(true);
        }


        private IEnumerator Half()
        {
            yield return null;

            lockButton.SetActive(true);
            layerButtons.gameObject.SetActive(true);
            navButtons.gameObject.SetActive(true);
            rotateZButtons.gameObject.SetActive(false);
            rotateYButtons.gameObject.SetActive(false);
        }


        private IEnumerator Closed()
        {
            yield return null;

            lockButton.SetActive(false);
            layerButtons.gameObject.SetActive(false);
            navButtons.gameObject.SetActive(false);
            rotateZButtons.gameObject.SetActive(false);
            rotateYButtons.gameObject.SetActive(false);
        }

        #endregion

        #region Event Handlers

        private void OnMenuChanged(GarageManager.Menus previousMenu, GarageManager.Menus currentMenu)
        {
            StopAllCoroutines();
            StartCoroutine(SwitchingMenu(currentMenu));
        }

        #endregion
    }
}