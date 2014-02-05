// Steve Yeager
// 12.4.2013

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using System;

/// <summary>
/// Activates weapons on Ship.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    #region Public Fields

    public Weapon[] weapons;
    public bool canActivate = true;

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds weapons.
    /// </summary>
    /// <param name="weaponList">Weapons to add by their index.</param>
    public void Bake(List<Weapon> weaponList)
    {
        if (weaponList == null || weaponList.Count == 0)
        {
            weapons = new Weapon[0];
        }
        else
        {
            int max = weaponList.Max(w => w.index);
            weapons = new Weapon[max+1];
        }

        foreach (var weapon in weaponList)
        {
            weapons[weapon.index] = weapon;
        }
    }


    /// <summary>
    /// Initialize all weapons.
    /// </summary>
    /// <param name="sender">Ship weapons are attached to.</param>
    public void Initialize(Ship sender)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].Initialize(sender);
            }
        }
    }


    /// <summary>
    /// See if weapon can be activated.
    /// </summary>
    /// <param name="weapon">Weapon index.</param>
    /// <returns>True, can activate.</returns>
    public bool CanActivate(int weapon)
    {
        if (weapon >= weapons.Length) return false;

        if (canActivate && weapons[weapon] != null)
        {
            return weapons[weapon].CanActivate();
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Activate weapon regardless if it can be activated or not.
    /// </summary>
    /// <param name="weapon">Weapon index.</param>
    /// <param name="isPressed">True, if weapon is pressed and not released.</param>
    public void Activate(int weapon, bool isPressed)
    {
        if (weapon >= weapons.Length) return;

        weapons[weapon].Activate(isPressed);
    }


    /// <summary>
    /// Activate weapon if it can be.
    /// </summary>
    /// <param name="weapon">Weapon index.</param>
    /// <param name="isPressed">True, if weapon is pressed and not released.</param>
    /// <returns>True, if the weapon is successfully activated.</returns>
    public bool TryActivate(int weapon, bool isPressed)
    {
        if (weapon >= weapons.Length) return false;
        if (!canActivate) return false;

        bool activated = CanActivate(weapon);
        if (activated)
        {
            Activate(weapon, isPressed);
        }

        return activated;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// TryActivate weapons registered to HUD button presses.
    /// </summary>
    /// <param name="sender">Button pressed.</param>
    /// <param name="args">Button args.</param>
    public void OnActivate(object sender, ActivateButtonArgs args)
    {
        TryActivate(int.Parse(args.value), args.isPressed);
    }

    #endregion
}