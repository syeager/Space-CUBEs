// Steve Yeager
// 4.1.2014

using UnityEngine;

/// <summary>
/// Disables PoolObjects OnExit.
/// </summary>
public class PoolObjectDestroyer : MonoBehaviour
{
    #region MonoBehaviour Overrides

    private void OnTriggerExit(Collider other)
    {
        PoolObject poolObject = other.GetComponent<PoolObject>();
        if (poolObject)
        {
            poolObject.Disable();
        }
    }

    #endregion
}