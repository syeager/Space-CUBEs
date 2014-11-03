// Little Byte Games

using Annotations;

namespace SpaceCUBEs
{
    /// <summary>
    /// Send hit information to parent.
    /// </summary>
    public class HealthChild : Health
    {
        #region Public Fields

        public Health parent;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Start()
        {
            myTransform = parent.myTransform;
        }

        public override float RecieveHit(Ship sender, float damage)
        {
            return parent.RecieveHit(sender, damage);
        }

        #endregion
    }
}