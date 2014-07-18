// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.04
// Edited: 2014.06.16

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Activates weapons on Ship.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    #region Public Fields

    public PlayerWeapon[] weapons;
    public bool canActivate = true;

    #endregion

    #region Private Fields

    public float DamageMultiplier { get; private set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds weapons.
    /// </summary>
    /// <param name="weaponList">Weapons to add by their index.</param>
    public void Bake(List<PlayerWeapon> weaponList)
    {
        weapons = new PlayerWeapon[BuildStats.ExpansionLimit];
        int weaponLimit = BuildStats.GetWeaponExpansion();

        foreach (PlayerWeapon weapon in weaponList.Where(weapon => weapon.index < weaponLimit))
        {
            weapons[weapon.index] = weapon;
        }
    }


    /// <summary>
    /// Initialize all weapons.
    /// </summary>
    /// <param name="sender">Ship weapons are attached to.</param>
    /// <param name="damageStat">Ship's damage stat. Used to create the damage multiplier. 1+damageStat/100.</param>
    public void Initialize(Ship sender, float damageStat)
    {
        DamageMultiplier = 1 + damageStat / 100;

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
    /// <returns></returns>
    public Coroutine Activate(int weapon, bool isPressed)
    {
        if (weapon >= weapons.Length) return null;
        if (weapons[weapon] == null) return null;

        return weapons[weapon].Activate(isPressed, DamageMultiplier);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="isPressed"></param>
    public void ActivateAll(bool isPressed)
    {
        foreach (PlayerWeapon weapon in weapons)
        {
            if (weapon == null) continue;

            weapon.Activate(isPressed, DamageMultiplier);
        }
    }


    /// <summary>
    /// Activate weapon if it can be.
    /// </summary>
    /// <param name="weapon">Weapon index.</param>
    /// <param name="isPressed">True, if weapon is pressed and not released.</param>
    /// <returns>True, if the weapon is successfully activated.</returns>
    public Coroutine TryActivate(int weapon, bool isPressed)
    {
        if (weapon >= weapons.Length) return null;
        if (!canActivate) return null;

        if (CanActivate(weapon))
        {
            return Activate(weapon, isPressed);
        }

        return null;
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