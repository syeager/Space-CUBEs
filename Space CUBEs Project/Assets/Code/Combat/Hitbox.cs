﻿// Little Byte Games

using UnityEngine;

namespace SpaceCUBEs
{
    // Space CUBEs Project-csharp
    // Author: Steve Yeager
    // Created: 2013.12.09
    // Edited: 2014.07.02
    
    /// <summary>
    /// Passes HitInfo from attack to reciever.
    /// </summary>
    public class Hitbox : MonoBase
    {
        #region References

        [SerializeField, HideInInspector]
        protected Transform myTransform;

        [SerializeField, HideInInspector]
        public PoolObject myPoolObject;

        #endregion

        #region Public Fields

        public bool continuous;
        public int hitNumber = 1;

        #endregion

        #region Protected Fields

        protected float damage;
        protected Ship sender;
        protected int hitCount;
        protected bool disabled;

        #endregion

        #region Const Fields

        protected const int PlayerLayer = 12;
        protected const int EnemyLayer = 13;

        #endregion

        #region MonoBehavoiur Overrides

        protected virtual void Awake()
        {
            myTransform = transform;
            myPoolObject = GetComponent<PoolObject>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (continuous || disabled) return;

            var oppHealth = other.gameObject.GetComponent(typeof(Health)) as Health;
            if (oppHealth != null)
            {
                // send damage
                oppHealth.RecieveHit(sender, damage);

                // disable if applicable
                if (hitNumber > 0)
                {
                    hitCount++;
                    if (hitCount >= hitNumber)
                    {
                        disabled = true;
                        myPoolObject.Disable();
                    }
                }
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!continuous) return;

            var oppHealth = other.gameObject.GetComponent(typeof(Health)) as Health;
            if (oppHealth != null)
            {
                oppHealth.RecieveHit(sender, damage * deltaTime);
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(Ship sender, float damage)
        {
            disabled = false;

            this.sender = sender;
            this.damage = damage;

            SetLayer(sender);
            hitCount = 0;
        }

        public virtual void Initialize(Ship sender, float damage, Vector3 moveVec)
        {
            Initialize(sender, damage);
            rigidbody.velocity = moveVec;
        }

        public virtual void Initialize(Ship sender, float damage, float time)
        {
            Initialize(sender, damage);
            myPoolObject.StartLifeTimer(time);
        }

        public virtual void Initialize(Ship sender, float damage, float time, Vector3 moveVec)
        {
            Initialize(sender, damage, time);
            rigidbody.velocity = moveVec;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets layer to PlayerWeapon or EnemyWeapon.
        /// </summary>
        /// <param name="sender">Ship that fired the weapon.</param>
        protected void SetLayer(Ship sender)
        {
            gameObject.layer = sender.CompareTag("Player") ? PlayerLayer : EnemyLayer;
        }

        #endregion
    }
}