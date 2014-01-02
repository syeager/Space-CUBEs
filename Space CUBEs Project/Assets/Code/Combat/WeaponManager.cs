// Steve Yeager
// 12.4.2013

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class WeaponManager : MonoBehaviour
{
    #region Public Fields

    public Weapon[] weapons;
    public bool canActivate = true;

    #endregion

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="weaponList"></param>
    public void Bake(List<Weapon> weaponList)
    {
        int max = weaponList.Max(w => w.index);
        weapons = new Weapon[max+1];

        foreach (var weapon in weaponList)
        {
            weapons[weapon.index] = weapon;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
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
    /// 
    /// </summary>
    public void RegisterToHUD()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            HUD.Main.WeaponButtons[i].ActivateEvent += OnActivate;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public bool CanActivate(int weapon)
    {
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
    /// 
    /// </summary>
    /// <param name="weapon"></param>
    /// <param name="isPressed"></param>
    public void Activate(int weapon, bool isPressed)
    {
        weapons[weapon].Activate(isPressed);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="weapon"></param>
    /// <param name="isPressed"></param>
    public void TryActivate(int weapon, bool isPressed)
    {
        if (CanActivate(weapon))
        {
            Activate(weapon, isPressed);
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnActivate(object sender, ActivateButtonArgs args)
    {
        TryActivate(int.Parse(args.value), args.isPressed);
    }

    #endregion
}