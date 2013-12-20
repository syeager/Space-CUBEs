// Steve Yeager
// 12.4.2013

using UnityEngine;

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


    public void RegisterToHUD()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            HUD.Main.WeaponButtons[i].ActivateEvent += OnActivate;
        }
    }


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


    public void Activate(int weapon, bool isPressed)
    {
        weapons[weapon].Activate(isPressed);
    }


    public void TryActivate(int weapon, bool isPressed)
    {
        if (CanActivate(weapon))
        {
            Activate(weapon, isPressed);
        }
    }

    #endregion

    #region Event Handlers

    private void OnActivate(object sender, ActivateButtonArgs args)
    {
        TryActivate(int.Parse(args.value), args.isPressed);
    }

    #endregion
}