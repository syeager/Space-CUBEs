﻿// Little Byte Games

using System;
using System.Collections;
using LittleByte.Extensions;
using UnityEngine;

namespace SpaceCUBEs
{
    public abstract class Weapon : MonoBase
    {
        #region References

        protected Transform myTransform;
        protected Ship myShip;

        #endregion

        #region Public Fields

        public new string name;

        /// <summary>Time in seconds to completely refill power gauge.</summary>
        public float cooldownTime;

        /// <summary>Power regenerated every second.</summary>
        public float cooldownSpeed { get; set; }

        public bool canActivate = true;

        #endregion

        #region Const Fields

        public const float FullPower = 100f;

        #endregion

        #region Properties

        public float power { get; protected set; }
        public int index { get; set; }

        #endregion

        #region Events

        public EventHandler<ValueArgs> PowerUpdateEvent;
        public EventHandler ActivatedEvent;
        public EventHandler<ValueArgs> EnabledEvent;

        #endregion

        #region Public Methods

        public Coroutine CoolDown()
        {
            return StartCoroutine(CoolingDown(true));
        }

        public void Enable()
        {
            canActivate = true;
            EnabledEvent.Fire(this, new ValueArgs(true));
        }

        public void Disable()
        {
            canActivate = false;
            EnabledEvent.Fire(this, new ValueArgs(false));
        }

        #endregion

        #region Protected Methods

        protected IEnumerator CoolingDown(bool empty = false, bool deActivate = true)
        {
            if (empty) power = 0f;
            if (deActivate)
            {
                ActivatedEvent.Fire();
                canActivate = false;
            }

            do
            {
                power += cooldownSpeed * deltaTime;
                if (PowerUpdateEvent != null)
                {
                    PowerUpdateEvent(this, new ValueArgs(power));
                }
                yield return null;
            } while (power < FullPower);

            power = FullPower;
            canActivate = true;
        }

        #endregion

        #region Virtual Methods

        public virtual void Initialize(Ship sender)
        {
            myTransform = transform;
            myShip = sender;
            power = FullPower;
            cooldownSpeed = FullPower / cooldownTime;
        }

        public virtual bool CanActivate()
        {
            return canActivate;
        }

        /// <summary>
        /// Activate weapon with extra info.
        /// </summary>
        /// <param name="pressed">Is the weapon being pressed?</param>
        public virtual Coroutine Activate(bool pressed)
        {
            return null;
        }

        #endregion
    }
}