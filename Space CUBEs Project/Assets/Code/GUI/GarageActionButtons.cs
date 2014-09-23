// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.16
// Edited: 2014.09.22

using System;
using System.Collections;
using System.Linq;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class GarageActionButtons : MonoBehaviour
    {
        #region Public Fields

        public ActionButtonGroup[] buttons;

        #endregion

        #region Private Fields

        [SerializeField, UsedImplicitly]
        private float speed;

        [SerializeField, UsedImplicitly]
        private UILabel pickupPlaceLabel;

        [SerializeField, UsedImplicitly]
        private int[] buttonPositions;

        #endregion

        #region Const Fields

        private const string Pickup = "Pickup";
        private const string Place = "Place";

        private const string PickupPlaceButton = "Pickup/Place Button";
        private const string DeleteButton = "Delete Button";

        private const string PaintAllButton = "Paint All Button";
        private const string PalleteButton = "Pallete Button";
        private const string SampleButton = "Sample Button";
        private const string PaintButton = "Paint Button";

        private const string WeaponButton = "Weapon Button";
        private const string AugButton = "Aug Button";

        #endregion

        #region Events

        public event Action PickupPlaceEvent;
        public event Action DeleteEvent;

        public event Action PaintAllEvent;
        public event Action PalleteEvent;
        public event Action SampleEvent;
        public event Action PaintEvent;

        public event Action WeaponEvent;
        public event Action AugEvent;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Start()
        {
            GarageManager.Main.MenuChangedEvent += OnMenuChanged;
            GarageManager.Main.grid.StatusChangedEvent += OnCursorStatusChanged;
            OnCursorStatusChanged(null, new CursorUpdatedArgs(ConstructionGrid.CursorStatuses.None, GarageManager.Main.grid.cursorStatus));
        }


        private void OnCursorStatusChanged(object sender, CursorUpdatedArgs args)
        {
            ConstructionGrid grid = (ConstructionGrid)sender;

            switch (args.current)
            {
                case ConstructionGrid.CursorStatuses.Holding:
                    // edit
                    buttons[0].buttons[1].isEnabled = true;
                    pickupPlaceLabel.text = Place;
                    buttons[0].buttons[0].isEnabled = true;
                    break;

                case ConstructionGrid.CursorStatuses.Hover:
                    // edit
                    buttons[0].buttons[1].isEnabled = true;
                    pickupPlaceLabel.text = Pickup;
                    buttons[0].buttons[0].isEnabled = false;
                    break;

                case ConstructionGrid.CursorStatuses.None:
                    // edit
                    buttons[0].buttons[1].isEnabled = false;
                    buttons[0].buttons[0].isEnabled = false;
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void ButtonPressed(UIButton button)
        {
            switch (button.name)
            {
                case PickupPlaceButton:
                    PickupPlaceEvent.Invoke();
                    break;
                case DeleteButton:
                    DeleteEvent.Invoke();
                    break;
                case PaintAllButton:
                    PaintAllEvent.Invoke();
                    break;
                case PalleteButton:
                    PalleteEvent.Invoke();
                    break;
                case SampleButton:
                    SampleEvent.Invoke();
                    break;
                case PaintButton:
                    PaintEvent.Invoke();
                    break;
                case WeaponButton:
                    WeaponEvent.Invoke();
                    break;
                case AugButton:
                    AugEvent.Invoke();
                    break;
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator SwitchingMenu(GarageManager.Menus previousMenu, GarageManager.Menus currentMenu)
        {
            int previous = (int)previousMenu;
            if (previous < buttons.Length)
            {
                yield return StartCoroutine(ClosingMenu(buttons[previous]));
            }

            int current = (int)currentMenu;
            if (current < buttons.Length)
            {
                yield return StartCoroutine(OpeningMenu(buttons[current]));
            }
        }


        private IEnumerator ClosingMenu(ActionButtonGroup group)
        {
            const float closePosition = 375f;
            Vector3 move = Vector3.right * speed;

            Transform[] buttonTansforms = group.buttons.Select(button => button.transform).ToArray();

            while (true)
            {
                float deltaTime = Time.deltaTime;
                for (int i = 0; i < buttonTansforms.Length; i++)
                {
                    if (buttonTansforms[i].localPosition.x == closePosition) continue;
                    if (buttonTansforms[i].localPosition.x > closePosition)
                    {
                        buttonTansforms[i].localPosition = new Vector3(closePosition, 0f, 0f);
                        buttonTansforms[i].gameObject.SetActive(false);
                        if (i == buttonTansforms.Length - 1)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        buttonTansforms[i].localPosition += move * deltaTime;
                    }
                }

                yield return null;
            }
        }


        private IEnumerator OpeningMenu(ActionButtonGroup group)
        {
            Vector3 move = Vector3.left * speed;

            Transform[] buttonTansforms = group.buttons.Select(button => button.transform).ToArray();
            foreach (Transform button in buttonTansforms)
            {
                button.gameObject.SetActive(true);
            }

            while (true)
            {
                float deltaTime = Time.deltaTime;
                for (int i = 0; i < buttonTansforms.Length; i++)
                {
                    if (buttonTansforms[i].localPosition.x == buttonPositions[i]) continue;
                    if (buttonTansforms[i].localPosition.x < buttonPositions[i])
                    {
                        buttonTansforms[i].localPosition = new Vector3(buttonPositions[i], 0f, 0f);
                        if (i == buttonTansforms.Length - 1)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        buttonTansforms[i].localPosition += move * deltaTime;
                    }
                }

                yield return null;
            }
        }

        #endregion

        #region Event Handlers

        private void OnMenuChanged(GarageManager.Menus previousMenu, GarageManager.Menus currentMenu)
        {
            StopAllCoroutines();
            StartCoroutine(SwitchingMenu(previousMenu, currentMenu));
        }

        #endregion

        #region Classes

        [Serializable]
        public class ActionButtonGroup
        {
            public UIButton[] buttons;
        }

        #endregion
    }
}