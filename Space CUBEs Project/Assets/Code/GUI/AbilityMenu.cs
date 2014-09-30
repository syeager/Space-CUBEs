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
        private ScrollviewButton extraButtonPrefab;

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
        private ActivateButton[] activeWeapons;

        private List<ActivateButton> extraWeapons;

        private ActivateButton selectedWeapon;

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
        private ActivateButton[] activeAugs;

        private List<ActivateButton> extraAugs;

        private ActivateButton selectedAug;

        private int? augIndex;

        #endregion

        #region Public Methods

        public void Initialize()
        {
            grid = GarageManager.Main.grid;

            actionButtons.WeaponEvent += OnWeaponPressed;
            actionButtons.AugEvent += OnAugPressed;

            extraWeapons = new List<ActivateButton>();
            extraAugs = new List<ActivateButton>();

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
                foreach (ActivateButton button in extraWeapons)
                {
                    Destroy(button.gameObject);
                }
                extraWeapons = new List<ActivateButton>();

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
            for (int i = 0; i < grid.weapons.Count; i++)
            {
                if (i < Player.Weaponlimit)
                {
                    activeWeapons[i].Initialize(grid.weapons[i].name, i.ToString());
                    activeWeapons[i].isEnabled = true;
                }
                else
                {
                    var extraWeapon = (ScrollviewButton)Instantiate(extraButtonPrefab);
                    extraWeapon.Initialize(i.ToString(), grid.weapons[i].name, i.ToString(), weaponGrid, weaponScrollview);
                    extraWeapon.ActivateEvent += OnActivatedWeaponPressed;
                    extraWeapons.Add(extraWeapon);
                }
            }

            // disable unused active buttons
            if (grid.weapons.Count < Player.Weaponlimit)
            {
                for (int i = grid.weapons.Count; i < Player.Weaponlimit; i++)
                {
                    activeWeapons[i].isEnabled = false;
                }
            }

            // augs
            for (int i = 0; i < grid.augmentations.Count; i++)
            {
                if (i < Player.Weaponlimit)
                {
                    activeAugs[i].Initialize(grid.augmentations[i].name, i.ToString());
                    activeAugs[i].isEnabled = true;
                }
                else
                {
                    var extraAug = (ScrollviewButton)Instantiate(extraButtonPrefab);
                    extraAug.Initialize(i.ToString(), grid.augmentations[i].name, i.ToString(), augGrid, augScrollview);
                    extraAug.ActivateEvent += OnActivatedAugPressed;
                    extraAugs.Add(extraAug);
                }
            }

            // disable unused active buttons
            if (grid.augmentations.Count < Player.Weaponlimit)
            {
                for (int i = grid.augmentations.Count; i < Player.Weaponlimit; i++)
                {
                    activeAugs[i].isEnabled = false;
                }
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

            int targetIndex = int.Parse(args.value);
            ActivateButton button = (ActivateButton)sender;

            if (selectedWeapon == null)
            {
                selectedWeapon = button;
                button.Activate(true);
                StartBlink(targetIndex, true);
                weaponIndex = targetIndex;
            }
            else if (selectedWeapon == button)
            {
                button.Activate(false);
                selectedWeapon = null;
                StopBlink(targetIndex, true);
                weaponIndex = null;
            }
            else
            {
                // swap
                int sourceIndex = int.Parse(selectedWeapon.value);
                grid.MoveWeaponMap(sourceIndex, targetIndex - sourceIndex);

                if (targetIndex >= Player.Weaponlimit)
                {
                    int extraIndex = targetIndex - Player.Weaponlimit;
                    extraWeapons[extraIndex].Initialize(grid.weapons[targetIndex].name, targetIndex.ToString());
                }
                else
                {
                    activeWeapons[targetIndex].Initialize(grid.weapons[targetIndex].name, targetIndex.ToString());
                }

                if (sourceIndex >= Player.Weaponlimit)
                {
                    int extraIndex = sourceIndex - Player.Weaponlimit;
                    extraWeapons[extraIndex].Initialize(grid.weapons[sourceIndex].name, sourceIndex.ToString());
                }
                else
                {
                    activeWeapons[sourceIndex].Initialize(grid.weapons[sourceIndex].name, sourceIndex.ToString());
                }

                button.Activate(false);
                selectedWeapon.Activate(false);

                Deselect(Abilities.Weapon);
                StopBlink(targetIndex, true);
                weaponIndex = null;
            }
        }


        private void OnActivatedAugPressed(object sender, ActivateButtonArgs args)
        {
            if (!args.isPressed) return;

            int targetIndex = int.Parse(args.value);
            ActivateButton button = (ActivateButton)sender;

            if (selectedAug == null)
            {
                selectedAug = button;
                button.Activate(true);
                StartBlink(targetIndex, false);
                augIndex = targetIndex;
            }
            else if (selectedAug == button)
            {
                button.Activate(false);
                selectedAug = null;
                StopBlink(targetIndex, false);
                augIndex = null;
            }
            else
            {
                // swap
                int sourceIndex = int.Parse(selectedAug.value);
                grid.MoveAugMap(sourceIndex, targetIndex - sourceIndex);

                if (targetIndex >= Player.Weaponlimit)
                {
                    int extraIndex = targetIndex - Player.Weaponlimit;
                    extraAugs[extraIndex].Initialize(grid.augmentations[targetIndex].name, targetIndex.ToString());
                }
                else
                {
                    activeAugs[targetIndex].Initialize(grid.augmentations[targetIndex].name, targetIndex.ToString());
                }

                if (sourceIndex >= Player.Weaponlimit)
                {
                    int extraIndex = sourceIndex - Player.Weaponlimit;
                    extraAugs[extraIndex].Initialize(grid.augmentations[sourceIndex].name, sourceIndex.ToString());
                }
                else
                {
                    activeAugs[sourceIndex].Initialize(grid.augmentations[sourceIndex].name, sourceIndex.ToString());
                }

                button.Activate(false);
                selectedAug.Activate(false);

                selectedAug = null;
                StopBlink(targetIndex, false);
                augIndex = null;
            }
        }

        #endregion
    }
}