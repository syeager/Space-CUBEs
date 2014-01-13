// Steve Yeager
// 1.14.2014

using UnityEngine;

public class Destroyer : MonoBehaviour
{
    #region MonoBehaviour Overrides

    private void OnTriggerEnter(Collider other)
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
            return;
        }
    }

    #endregion
}