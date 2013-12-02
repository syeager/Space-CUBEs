// Steve Yeager
// 12.01.2013

using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Public Fields

    /// <summary>1 per second.</summary>
    public float cooldownSpeed;

    #endregion

    #region Protected Fields

    protected float power;

    #endregion

    #region Const Fields

    protected const float FULLPOWER = 100f;

    #endregion


    #region Virtual Methods

    public virtual bool CanActivate()
    {
        return false;
    }


    public virtual void Activate()
    {

    }

    #endregion

    #region Protected Methods

    protected IEnumerator Cooldown()
    {
        while (power < FULLPOWER)
        {
            power += cooldownSpeed*Time.deltaTime;
            yield return null;
        }

        power = FULLPOWER;
    }

    #endregion
}