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


    public void Activate(int weapon)
    {
        weapons[weapon].Activate();
    }

    #endregion
}