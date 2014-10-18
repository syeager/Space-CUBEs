// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.17
// Edited: 2014.10.17

using UnityEngine;

namespace SpaceCUBEs
{
    public class AbilityButton : ScrollviewButton
    {
        #region Properties

        public int AbilityIndex { get; private set; }
        public int ButtonIndex { get; private set; }
        public bool IsExtra { get; private set; }

        #endregion

        #region Scrollview Buttons 

        public void Initialize(string label, int abilityIndex, int buttonIndex, bool isExtra, Transform parent = null, UIScrollView scrollView = null)
        {
            this.label.text = label;
            Set(abilityIndex, buttonIndex, isExtra);
            
            if (scrollView != null) dragScrollView.scrollView = scrollView;
            if (parent != null)
            {
                transform.parent = parent;
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
            }
        }

        #endregion

        #region Public Methods

        public void Set(int weaponIndex, int buttonIndex, bool isExtra)
        {
            AbilityIndex = weaponIndex;
            ButtonIndex = buttonIndex;
            IsExtra = isExtra;
        }

        #endregion
    } 
}