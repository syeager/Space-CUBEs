// Steve Yeager
// 12.8.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// Passes HitInfo from attack to reciever.
/// </summary>
[RequireComponent(typeof(PoolObject))]
public class Hitbox : MonoBase
{
    #region References

    private Transform myTransform;
    private PoolObject myPoolObject;

    #endregion

    #region Public Fields

    public bool continuous;
    public int hitNumber = 1;

    #endregion

    #region Private Fields

    private HitInfo hitInfo;
    private Ship sender;
    private int hitCount;

    #endregion


    #region MonoBehavoiur Overrides

    private void Awake()
    {
        myTransform = transform;
        myPoolObject = GetComponent<PoolObject>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (continuous) return;

        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, hitInfo);
            if (hitNumber > 0)
            {
                hitCount++;
                if (hitCount >= hitNumber)
                {
                    GetComponent<PoolObject>().Disable();
                }
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!continuous) return;

        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, new HitInfo { damage = hitInfo.damage * deltaTime });
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(Ship sender, HitInfo hitInfo)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;

        gameObject.layer = sender.gameObject.layer;
        hitCount = 0;
    }


    public void Initialize(Ship sender, HitInfo hitInfo, float time)
    {
        Initialize(sender, hitInfo);
        myPoolObject.StartLifeTimer(time);
    }


    public void Initialize(Ship sender, HitInfo hitInfo, float time, Vector3 moveVec)
    {
        StartCoroutine(Move(moveVec));
        Initialize(sender, hitInfo, time);
    }

    #endregion

    #region Private Methods

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