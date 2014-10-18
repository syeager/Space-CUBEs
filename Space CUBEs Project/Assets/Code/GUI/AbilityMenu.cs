// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.23
// Edited: 2014.09.29

using System.Collections.Generic;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class AbilityMenu : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private GarageActionButtons actionButtons;

        [SerializeField, UsedImplicitly]
        private AbilityButton extraButtonPrefab;

        private ConstructionGrid grid;

        private enum Abilities
        {
            Weapon,
            Aug,
        }

        private Abilities openMenu;

        #endregion

        #region Weapons

        [Header("Weapons")]
        [SerializeField, UsedImplicitly]
        private GameObject weaponMenu;

        [SerializeField, UsedImplicitly]
        private UIScrollView weaponScrollview;

        [SerializeField, UsedImplicitly]
        private Transform weaponGrid;

        [SerializeField, UsedImplicitly]
        private AbilityButton[] activeWeapons;

        private List<AbilityButton> extraWeapons;

        private AbilityButton selectedWeapon;

        private int? weaponIndex;

        #endregion

        #region Weapons

        [Header("Augs")]
        [SerializeField, UsedImplicitly]
        private GameObject augMenu;

        [SerializeField, UsedImplicitly]
        private UIScrollView augScrollview;

        [SerializeField, UsedImplicitly]
        private Transform augGrid;

        [SerializeField, UsedImplicitly]
        private AbilityButton[] activeAugs;

        private List<AbilityButton> extraAugs;

        private AbilityButton selectedAug;

        private int? augIndex;

        #endregion

        #region Public Methods

        public void Initialize()
        {
            grid = GarageManager.Main.grid;

            actionButtons.WeaponEvent += OnWeaponPressed;
            actionButtons.AugEvent += OnAugPressed;

            extraWeapons = new List<AbilityButton>();
            extraAugs = new List<AbilityButton>();

            openMenu = Abilities.Weapon;

            RegisterActive();
        }


        public void Activate(bool activate)
        {
            if (activate)
            {
                CreateButtons();
                gameObject.SetActive(true);

                EnableActionButton(openMenu);
            }
            else
            {
                gameObject.SetActive(false);
                foreach (AbilityButton button in extraWeapons)
                {
                    Destroy(button.gameObject);
                }
                extraWeapons = new List<AbilityButton>();

                Deselect(openMenu);
            }
        }

        #endregion

        #region Private Methods

        private void RegisterActive()
        {
            for (int i = 0; i < Player.Weaponlimit; i++)
            {
                activeWeapons[i].ActivateEvent += OnActivatedWeaponPressed;
                activeAugs[i].ActivateEvent += OnActivatedAugPressed;
            }
        }


        private void CreateButtons()
        {
            // weapons
            int weaponLevel = BuildStats.GetWeaponLevel();
            for (int i = 0; i < grid.weapons.Count; i++)
            {
                if (i <= weaponLevel)
                {
                    activeWeapons[i].Initialize(grid.weapons[i].name, i, i, false);
                    activeWeapons[i].isEnabled = true;
                }
                else
                {
                    var extraWeapon = (AbilityButton)Instantiate(extraButtonPrefab);
                    extraWeapon.Initialize(grid.weapons[i].name, i, extraWeapons.Count, true, weaponGrid, weaponScrollview);
                    extraWeapon.ActivateEvent += OnActivatedWeaponPressed;
                    extraWeapons.Add(extraWeapon);
                }
            }

            // disable unused active buttons
            for (int i = 0; i < Player.Weaponlimit; i++)
            {
                activeWeapons[i].isEnabled = i <= weaponLevel;
            }

            // augs
            int augLevel = BuildStats.GetAugmentationLevel();
            for (int i = 0; i < grid.augmentations.Count; i++)
            {
                if (i <= augLevel)
                {
                    activeAugs[i].Initialize(grid.augmentations[i].name, i, i, false);
                    activeAugs[i].isEnabled = true;
                }
                else if (i < Player.Weaponlimit)
                {
                    var extraAug = (AbilityButton)Instantiate(extraButtonPrefab);
                    extraAug.Initialize(grid.augmentations[i].name, i, extraAugs.Count, true, augGrid, augScrollview);
                    extraAug.ActivateEvent += OnActivatedAugPressed;
                    extraAugs.Add(extraAug);
                }
            }

            // disable unused active buttons
            for (int i = 0; i < Player.Weaponlimit; i++)
            {
                activeAugs[i].isEnabled = i <= augLevel;
            }
        }


        private void StartBlink(int index, bool weapon)
        {
            StopBlink(index, weapon);
            grid.StartBlink(grid.weapons[index].renderer);
        }


        private void StopBlink(int index, bool weapon)
        {
            if (weapon)
            {
                grid.StopBlink(grid.weapons[index].renderer);
            }
            else
            {
                grid.StopBlink(grid.augmentations[index].renderer);
            }
        }


        private void Deselect(Abilities ability)
        {
            if (ability == Abilities.Weapon)
            {
                if (weaponIndex != null)
                {
                    StopBlink(weaponIndex.Value, true);
                }
                if (selectedWeapon != null)
                {
                    selectedWeapon.Activate(false);
                    selectedWeapon = null;
                }
            }
            else
            {
                if (augIndex != null)
                {
                    StopBlink(augIndex.Value, true);
                }
                if (selectedAug != null)
                {
                    selectedAug.Activate(false);
                    selectedAug = null;
                }
            }
        }


        private void EnableActionButton(Abilities ability)
        {
            actionButtons.buttons[2].buttons[0].isEnabled = ability != Abilities.Weapon;
            actionButtons.buttons[2].buttons[1].isEnabled = ability != Abilities.Aug;
        }

        #endregion

        #region Event Handlers

        private void OnWeaponPressed()
        {
            openMenu = Abilities.Weapon;
            weaponMenu.SetActive(true);
            augMenu.SetActive(false);
            Deselect(Abilities.Aug);
            EnableActionButton(openMenu);
        }


        private void OnAugPressed()
        {
            openMenu = Abilities.Aug;
            weaponMenu.SetActive(false);
            augMenu.SetActive(true);
            Deselect(Abilities.Weapon);
            EnableActionButton(openMenu);
        }


        private void OnActivatedWeaponPressed(object sender, ActivateButtonArgs args)
        {
            if (!args.isPressed) return;

            AbilityButton clickedButton = (AbilityButton)sender;

            if (selectedWeapon == null)
            {
                selectedWeapon = clickedButton;
                clickedButton.Activate(true);
                StartBlink(clickedButton.AbilityIndex, true);
                weaponIndex = clickedButton.AbilityIndex;
            }
            else if (selectedWeapon == clickedButton)
            {
                clickedButton.Activate(false);
                selectedWeapon = null;
                StopBlink(clickedButton.AbilityIndex, true);
                weaponIndex = null;
            }
            else
            {
                // swap
                grid.MoveWeaponMap(selectedWeapon.AbilityIndex, clickedButton.AbilityIndex - selectedWeapon.AbilityIndex);

                // moving to extra
                if (clickedButton.IsExtra)
                {
                    extraWeapons[clickedButton.ButtonIndex].Initialize(grid.weapons[clickedButton.AbilityIndex].name, clickedButton.AbilityIndex, clickedButton.ButtonIndex, true);
                }
                else
                {
                    activeWeapons[clickedButton.ButtonIndex].Initialize(grid.weapons[clickedButton.AbilityIndex].name, clickedButton.AbilityIndex, clickedButton.ButtonIndex, false);
                }

                // moving from extra
                if (selectedWeapon.IsExtra)
                {
                    extraWeapons[selectedWeapon.ButtonIndex].Initialize(grid.weapons[selectedWeapon.AbilityIndex].name, selectedWeapon.AbilityIndex, selectedWeapon.ButtonIndex, true);
                }
                else
                {
                    activeWeapons[selectedWeapon.ButtonIndex].Initialize(grid.weapons[selectedWeapon.AbilityIndex].name, selectedWeapon.AbilityIndex, selectedWeapon.ButtonIndex, false);
                }

                clickedButton.Activate(false);
                selectedWeapon.Activate(false);

                Deselect(Abilities.Weapon);
                StopBlink(clickedButton.AbilityIndex, true);
                weaponIndex = null;
            }
        }


        private void OnActivatedAugPressed(object sender, ActivateButtonArgs args)
        {
            if (!args.isPressed) return;

            AbilityButton clickedButton = (AbilityButton)sender;

            if (selectedAug == null)
            {
                selectedAug = clickedButton;
                clickedButton.Activate(true);
                StartBlink(clickedButton.AbilityIndex, true);
                augIndex = clickedButton.AbilityIndex;
            }
            else if (selectedAug == clickedButton)
            {
                clickedButton.Activate(false);
                selectedAug = null;
                StopBlink(clickedButton.AbilityIndex, true);
                augIndex = null;
            }
            else
            {
                // swap
                grid.MoveAugMap(selectedAug.AbilityIndex, clickedButton.AbilityIndex - selectedAug.AbilityIndex);

                // moving to extra
                if (clickedButton.IsExtra)
                {
                    extraAugs[clickedButton.ButtonIndex].Initialize(grid.augmentations[clickedButton.AbilityIndex].name, clickedButton.AbilityIndex, clickedButton.ButtonIndex, true);
                }
                else
                {
                    activeAugs[clickedButton.ButtonIndex].Initialize(grid.augmentations[clickedButton.AbilityIndex].name, clickedButton.AbilityIndex, clickedButton.ButtonIndex, false);
                }

                // moving from extra
                if (selectedAug.IsExtra)
                {
                    extraAugs[selectedAug.ButtonIndex].Initialize(grid.augmentations[selectedAug.AbilityIndex].name, selectedAug.AbilityIndex, selectedAug.ButtonIndex, true);
                }
                else
                {
                    activeAugs[selectedAug.ButtonIndex].Initialize(grid.augmentations[selectedAug.AbilityIndex].name, selectedAug.AbilityIndex, selectedAug.ButtonIndex, false);
                }

                clickedButton.Activate(false);
                selectedAug.Activate(false);

                Deselect(Abilities.Aug);
                StopBlink(clickedButton.AbilityIndex, true);
                augIndex = null;
            }
        }

        #endregion
    }
}