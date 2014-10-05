// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.10.04

using System.Collections.Generic;
using Annotations;
using UnityEngine;

namespace LittleByte.NGUI
{
    public class SelectableButton : ActivateButton
    {
        #region Public Fields

        public string group;

        public bool startSelected;

        public bool toggle;

        public enum ActivateTypes
        {
            Press,
            Release,
            Click
        }

        public ActivateTypes activateType;

        #endregion

        #region Private Fields

        private bool selected;

        [SerializeField, UsedImplicitly]
        private UIDragScrollView dragScrollView;

        #endregion

        #region Static Fields

        private static readonly Dictionary<string, SelectableButton> SelectedButtons = new Dictionary<string, SelectableButton>();

        #endregion

        #region MonoBehaviour Overrides

        protected virtual void Awake()
        {
            // initialize groups
            if (!SelectedButtons.ContainsKey(group))
            {
                SelectedButtons.Add(group, null);
            }

            if (startSelected)
            {
                SetSelected(this);
            }
        }

        #endregion

        #region UIButton Overrides

        protected override void OnPress(bool isPressed)
        {
            if (activateType == ActivateTypes.Click) return;
            if (activateType == ActivateTypes.Press && !isPressed) return;
            if (activateType == ActivateTypes.Release && isPressed) return;

            if (SelectedButtons[group] == this)
            {
                if (toggle)
                {
                    // deselect
                    base.OnPress(isPressed);
                    Deselect(this);
                }
            }
            else
            {
                // select
                base.OnPress(isPressed);
                SetSelected(this);
            }
        }


        protected override void OnClick()
        {
            if (activateType != ActivateTypes.Click) return;
            base.OnClick();

            if (SelectedButtons[group] == this)
            {
                if (toggle)
                {
                    // deselect
                    base.OnClick();
                    Deselect(this);
                }
            }
            else
            {
                // select
                base.OnClick();
                SetSelected(this);
            }
        }

        #endregion

        #region Public Methods

        public void SetScrollView(string name, string label, string value, Transform parent, UIScrollView scrollView)
        {
            this.name = name;
            this.label.text = label;
            this.value = value;
            if (dragScrollView != null)
            {
                dragScrollView.scrollView = scrollView;
            }
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        #endregion

        #region Static Methods

        public static void SetSelected(SelectableButton button)
        {
            // no group
            if (string.IsNullOrEmpty(button.group))
            {
                button.enabled = false;
                button.SetState(State.Pressed, true);
                return;
            }

            // switch selected
            SelectableButton currentlySelected = SelectedButtons[button.group];
            if (currentlySelected != null)
            {
                currentlySelected.enabled = true;
                currentlySelected.SetState(State.Normal, true);
            }
            button.enabled = false;
            button.SetState(State.Pressed, true);
            SelectedButtons[button.group] = button;
        }


        public static void Deselect(SelectableButton button)
        {
            button.enabled = true;
            button.SetState(State.Normal, true);

            if (string.IsNullOrEmpty(button.group))
            {
                SelectedButtons[button.group] = null;
            }
        }

        #endregion
    }
}