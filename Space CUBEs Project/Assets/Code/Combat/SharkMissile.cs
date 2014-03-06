// Steve Yeager
// 3.4.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class SharkMissile : Hitbox
{
    #region Private Fields
    
    private float delaySpeed;
    private float homingSpeed;
    private Transform target;
    
    #endregion


    #region MonoBehaviour Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.transform != target) return;

        var oppHealth = other.gameObject.GetComponent<Health>();
        oppHealth.RecieveHit(sender, hitInfo);

        Destroy(gameObject);
    }

    #endregion

    #region Hitbox Overrides

    public void Initialize(Ship sender, HitInfo hitInfo, Transform target, float delay, float delaySpeed, float homingSpeed)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;
        this.delaySpeed = delaySpeed;
        this.homingSpeed = homingSpeed;
        this.target = target;

        gameObject.layer = sender.gameObject.layer;
        collider.enabled = false;

        if (target != null)
        {
            target.GetComponent<ShieldHealth>().DieEvent += OnTargetDeath;
        }

        StartCoroutine(Delay(delay));
    }

    #endregion

    #region Private Methods

    private IEnumerator Delay(float time)
    {
        while (time > 0f)
        {
            time -= deltaTime;

            myTransform.Translate(Vector3.forward * delaySpeed * deltaTime);

            yield return null;
        }

        if (target != null)
        {
            collider.enabled = true;
            StartCoroutine(Homing());
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator Homing()
    {
        while (true)
        {
            // rotate towards
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position-myTransform.position, Vector3.back), homingSpeed*0.5f*deltaTime);

            // move towards
            myTransform.Translate(Vector3.forward * homingSpeed * deltaTime);

            yield return null;
        }
    }

    #endregion

    #region Event Handlers

    private void OnTargetDeath(object sender, DieArgs args)
    {
        (sender as ShieldHealth).DieEvent -= OnTargetDeath;

        Destroy(gameObject);
    }

    #endregion
}