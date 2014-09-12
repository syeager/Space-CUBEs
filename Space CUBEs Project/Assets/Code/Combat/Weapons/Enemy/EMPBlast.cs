// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.05
// Edited: 2014.09.08

using UnityEngine;
using System.Collections;

namespace SpaceCUBEs
{
    /// <summary>
    /// 
    /// </summary>
    public class EMPBlast : Hitbox
    {
        #region MonoBehaviour Overrides

        protected override void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponent(typeof(Player)) as Player;
            if (player != null && !player.MyHealth.invincible)
            {
                foreach (PlayerWeapon playerWeapon in player.Weapons.weapons)
                {
                    playerWeapon.CoolDown();
                }

                return;
            }

            IEMPBlastListener listener = other.GetComponent(typeof(IEMPBlastListener)) as IEMPBlastListener;
            if (listener != null)
            {
                listener.InteractEMP();
            }
        }

        #endregion

        #region Hitbox Overrides

        public void Initialize(Ship sender, float damage, float time, float spreadSpeed)
        {
            Initialize(sender, damage, time);
            myTransform.localScale = Vector3.one;
            StartCoroutine(Spread(new Vector3(spreadSpeed, 0f, spreadSpeed)));
        }

        #endregion

        #region Private Methods

        private IEnumerator Spread(Vector3 scale)
        {
            while (true)
            {
                myTransform.localScale += scale * deltaTime;
                yield return null;
            }
        }

        #endregion
    }
}