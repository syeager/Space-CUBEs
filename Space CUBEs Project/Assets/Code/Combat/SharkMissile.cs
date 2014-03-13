// Steve Yeager
// 3.4.2014

using UnityEngine;
using System.Collections;

public class SharkMissile : Hitbox
{
    #region Private Fields
    
    private float homingSpeed;
    private Transform target;
    
    #endregion


    #region MonoBehaviour Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.transform != target) return;

        var oppHealth = other.gameObject.GetComponent<Health>();
        oppHealth.RecieveHit(sender, hitInfo);

        Detonate();
    }

    #endregion

    #region Hitbox Overrides

    public void Initialize(Ship sender, HitInfo hitInfo, float delay, float delaySpeed, float homingSpeed)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;
        this.homingSpeed = homingSpeed;
        target = null;

        gameObject.layer = sender.gameObject.layer;
        collider.enabled = false;

        StartCoroutine(Delay(delay, delaySpeed));
    }

    #endregion

    #region Private Methods

    private IEnumerator Delay(float time, float speed)
    {
        while (time > 0f)
        {
            time -= deltaTime;

            myTransform.Translate(Vector3.forward * speed * deltaTime);

            yield return null;
        }

        // find target
        FindTarget();
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


    private void Detonate()
    {
        if (!gameObject.activeSelf) return;

        target = null;
        StopAllCoroutines();
        GetComponent<PoolObject>().Disable();
    }


    private void FindTarget()
    {
        float max = 0f;
        foreach (var enemy in LevelManager.Main.activeEnemies)
        {
            if (enemy.GetComponent<ShieldHealth>().strength > max)
            {
                max = enemy.GetComponent<ShieldHealth>().strength;
                target = enemy.transform;
            }
        }

        if (target != null)
        {
            target.GetComponent<ShieldHealth>().DieEvent += OnTargetDeath;
            collider.enabled = true;
            StartCoroutine(Homing());
        }
        else
        {
            Detonate();
        }
    }

    #endregion

    #region Event Handlers

    private void OnTargetDeath(object sender, DieArgs args)
    {
        (sender as Health).DieEvent -= OnTargetDeath;
        StopAllCoroutines();

        if (target != null)
        {
            FindTarget();
        }
    }

    #endregion
}