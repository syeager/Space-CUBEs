// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.09.23

using System.Collections.Generic;
using Annotations;
using UnityEngine;

namespace LittleByte.NGUI
{
    public class SelectableButton : ActivateButton
    {
        #region Public Fields

        public int group = -1;

        #endregion

        #region Private Fields

        private bool selected;

        [SerializeField, UsedImplicitly]
        private UIDragScrollView dragScrollView;

        #endregion

        #region Static Fields

        private static readonly List<SelectableButton> SelectedButtons = new List<SelectableButton>();

        #endregion

        #region UIButton Overrides

        protected override void OnClick()
        {
            base.OnClick();
            SetSelected(this);
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
            if (button.group == -1)
            {
                button.enabled = false;
                button.SetState(State.Pressed, true);
                return;
            }

            // initialize groups
            while (SelectedButtons.Count - 1 < button.group)
            {
                SelectedButtons.Add(null);
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

        #endregion
    }
}