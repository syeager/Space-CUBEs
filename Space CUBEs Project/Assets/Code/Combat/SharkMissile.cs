// Steve Yeager
// 3.4.2014


using UnityEngine;
using System.Collections;

public class SharkMissile : Hitbox
{
    #region Public Fields

    public float rotationSpeed;

    #endregion

    #region Private Fields

    private float homingSpeed;
    private Transform target;
    
    #endregion


    #region MonoBehaviour Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth == null || oppHealth.myTransform != target) return;

        oppHealth.RecieveHit(sender, damage);
        Detonate();
    }

    #endregion

    #region Hitbox Overrides

    public void Initialize(Ship sender, float damage, float delay, float delaySpeed, float homingSpeed)
    {
        this.sender = sender;
        this.damage = damage;
        this.homingSpeed = homingSpeed;
        target = null;

        SetLayer(sender);
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
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position, Vector3.back), rotationSpeed * deltaTime);

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
        foreach (var enemy in LevelManager.Main.ActiveEnemies)
        {
            if (enemy.GetComponent<ShieldHealth>().Strength > max)
            {
                max = enemy.GetComponent<ShieldHealth>().Strength;
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