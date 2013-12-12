// Steve Yeager
// 12.8.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// Passes HitInfo from attack to reciever.
/// </summary>
public class Hitbox : MonoBase
{
    #region References

    private Transform myTransform;
    private PoolObject myPoolObject;

    #endregion

    #region Private Fields

    private HitInfo hitInfo;
    private Ship sender;
    private int ID;

    #endregion

    #region Static Fields

    private static int IDs;

    #endregion

    #region MonoBehavoiur Overrides

    private void Awake()
    {
        myTransform = transform;
        myPoolObject = GetComponent<PoolObject>();
    }


    private void OnTriggerStay(Collider other)
    {
        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, ID, hitInfo);
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(Ship sender, HitInfo hitInfo, float time)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;

        gameObject.layer = sender.gameObject.layer;
        UpdateID();

        myPoolObject.StartLifeTimer(time);
    }


    public void Initialize(Ship sender, HitInfo hitInfo, float time, Vector3 moveVec)
    {
        StartCoroutine(Move(moveVec));
        Initialize(sender, hitInfo, time);
    }

    #endregion

    #region Private Methods

    private void UpdateID()
    {
        IDs++;
        ID = IDs;
    }


    private IEnumerator Move(Vector3 moveVec)
    {
        while (true)
        {
            myTransform.Translate(moveVec, Space.World);
            yield return null;
        }
    }

    #endregion
}