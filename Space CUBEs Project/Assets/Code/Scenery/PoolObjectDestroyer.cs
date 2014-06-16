// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.27
// Edited: 2014.06.15

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