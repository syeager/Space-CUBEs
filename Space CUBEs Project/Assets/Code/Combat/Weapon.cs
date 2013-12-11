// Steve Yeager
// 12.01.2013

using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region References

    protected Transform myTransform;
    protected Ship myShip;

    #endregion

    #region Public Fields

    /// <summary>1 per second.</summary>
    public float cooldownSpeed;

    #endregion

    #region Protected Fields

    protected float power;
    protected bool canActivate = true;

    #endregion

    #region Const Fields

    protected const float FULLPOWER = 100f;

    #endregion


    #region MonoBehaviours

    private void Awake()
    {
        // get references
        myTransform = transform;
    }

    #endregion

    #region Public Methods

    public void Initialize(Ship sender)
    {
        myShip = sender;
    }

    #endregion

    #region Virtual Methods

    public virtual bool CanActivate()
    {
        return true;
    }


    public virtual void Activate(bool pressed)
    {

    }

    #endregion

    #region Protected Methods

    protected IEnumerator Cooldown()
    {
        canActivate = false;
        while (power < FULLPOWER)
        {
            power += cooldownSpeed*Time.deltaTime;
            yield return null;
        }

        power = FULLPOWER;
        canActivate = true;
    }

    #endregion
}