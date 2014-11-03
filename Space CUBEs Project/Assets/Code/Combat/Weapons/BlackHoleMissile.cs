// Little Byte Games

namespace SpaceCUBEs
{
    // Little Byte Games
    // Author: Steve Yeager
    // Created: 2014.02.20
    // Edited: 2014.08.16

    /// <summary>
    /// Creates a black hole when released.
    /// </summary>
    public class BlackHoleMissile : Hitbox, IEMPBlastListener
    {
        #region Public Fields

        public PoolObject blackHolePrefab;
        public float explosionTime;

        #endregion

        #region IEMPBlastListener

        public void InteractEMP()
        {
            myPoolObject.Disable();
        }

        #endregion

        #region Public Methods

        public void Explode()
        {
            Prefabs.Pop(blackHolePrefab, myTransform.position, myTransform.rotation).GetComponent<BlackHole>().Initialize(sender, damage, explosionTime);
            myPoolObject.Disable();
        }

        #endregion
    }
}