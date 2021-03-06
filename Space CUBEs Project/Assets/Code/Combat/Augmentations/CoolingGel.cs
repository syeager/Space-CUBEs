﻿// Steve Yeager
// 4.24.2014

using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Speeds up weapon cooldowns.
    /// </summary>
    public class CoolingGel : Augmentation
    {
        #region Public Fields

        /// <summary>Multiplies weapon's cooldown speed.</summary>
        public float cooldownBoost = 1.25f;

        #endregion


        #region Augmentation Methods

        public override void Initialize(Player player)
        {
            for (int i = 0; i < player.Weapons.weapons.Length; i++)
            {
                if (player.Weapons.weapons[i] != null)
                {
                    player.Weapons.weapons[i].cooldownSpeed *= cooldownBoost;
                }
            }
        }


        public override Augmentation Bake(GameObject player)
        {
            CoolingGel comp = player.AddComponent<CoolingGel>();
            comp.index = index;
            comp.cooldownBoost = cooldownBoost;

            return comp;
        }

        #endregion
    } 
}