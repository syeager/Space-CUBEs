// Little Byte Games

using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    // Steve Yeager
    // 1.14.2014
    
    public class Destroyer : MonoBehaviour
    {
        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void OnTriggerExit(Collider other)
        {
            ShieldHealth health = other.GetComponent<ShieldHealth>();
            if (health != null)
            {
                health.Trash();
                return;
            }

            PoolObject po = other.GetComponent<PoolObject>();
            if (po != null)
            {
                po.Disable();
            }
        }

        #endregion
    }
}