// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.16
// Edited: 2014.09.16

using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    using LittleByte;

    public class GarageNavMenu : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private UIButton activeButton;

        [SerializeField, UsedImplicitly]
        private UILabel activeLabel;

        [SerializeField, UsedImplicitly]
        private string menu = "Menu";

        [SerializeField, UsedImplicitly]
        private GarageMenuButton[] menuButtons;

        [SerializeField, UsedImplicitly]
        private GarageMenuButton exitButton;

        private bool open;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Awake()
        {
            menuButtons[0].isEnabled = false;
            activeLabel.text = GarageManager.Menus.Edit.ToString();
        }

        #endregion
        
        #region Public Methods

        public void ToggleOpen()
        {
            Toggle(!open);
        }


        public void SetMenu(GarageManager.Menus menu)
        {
            menuButtons[(int)GarageManager.Main.OpenMenu].isEnabled = true;
            menuButtons[(int)menu].isEnabled = false;
            
            Toggle(false);

            GarageManager.Main.ChangeMenu(menu);
        }


        public void Exit()
        {
            GarageManager.Main.Exit();
        }

        #endregion

        #region Private Methods

        private void Toggle(bool open)
        {
            this.open = open;
            activeLabel.text = open ? menu : GarageManager.Main.OpenMenu.ToString();

            foreach (var menuButton in menuButtons)
            {
                menuButton.Open(open);
            }
            exitButton.Open(open);
        }

        #endregion
    }
}