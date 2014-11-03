// Little Byte Games

using UnityEngine;

namespace SpaceCUBEs
{
    // Little Byte Games
    // Author: Steve Yeager
    // Created: 2014.03.03
    // Edited: 2014.08.16
    
    /// <summary>
    /// Giant nuke for the CBomb weapon.
    /// </summary>
    public class CBomb : Hitbox, IEMPBlastListener
    {
        #region Public Fields

        public PoolObject explosionPrefab;
        public float growTime;
        public float explosionLength;
        public float shrinkTime;

        #endregion

        #region MonoBehavoiur Overrides

        protected override void OnTriggerEnter(Collider other)
        {
            if (disabled) return;

            base.OnTriggerEnter(other);

            Detonate();
        }

        #endregion

        #region IEMPBlastListener Overrides

        public void InteractEMP()
        {
            disabled = true;
            GetComponent<PoolObject>().Disable();
        }

        #endregion

        #region Private Methods

        private void Detonate()
        {
            Prefabs.Pop(explosionPrefab, myTransform.position, myTransform.rotation).GetComponent<CBombExplosion>().Initialize(sender, damage, growTime, explosionLength, shrinkTime);

            GetComponent<PoolObject>().Disable();
        }

        #endregion
    }
}