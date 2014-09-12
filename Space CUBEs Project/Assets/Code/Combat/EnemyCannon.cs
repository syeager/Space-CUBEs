// Little Byte Games
// Author: Steve Yeager
// Created: 2014.03.27
// Edited: 2014.09.08

using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Fires shot at player.
    /// </summary>
    public class EnemyCannon : Weapon
    {
        #region Public Fields

        public PoolObject bulletPrefab;
        public float speed;
        public float damage;
        public Vector3 offset;

        #endregion

        #region Weapon Overrides

        public override Coroutine Activate(bool pressed)
        {
            if (pressed)
            {
                Fire();
            }

            return null;
        }

        #endregion

        #region Private Methods

        private void Fire()
        {
            Vector3 position = myTransform.position + myTransform.TransformDirection(offset);
            Prefabs.Pop(bulletPrefab, position, myTransform.rotation).
                    GetComponent<Hitbox>().Initialize(myShip, damage, (LevelManager.Main.PlayerTransform.position - position).normalized * speed);
        }

        #endregion
    }
}