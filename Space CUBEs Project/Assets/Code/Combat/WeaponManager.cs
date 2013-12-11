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


    public void Activate(int weapon, bool pressed)
    {
        weapons[weapon].Activate(pressed);
    }

    #endregion
}