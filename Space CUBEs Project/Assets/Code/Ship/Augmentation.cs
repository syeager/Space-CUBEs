// Steve Yeager
// 4.15.2014

using UnityEngine;

/// <summary>
/// Empty base class for augmentation CUBEs.
/// </summary>
public abstract class Augmentation : MonoBehaviour
{
    #region Abstract Methods

    public abstract void Initialize(Player player);

    public abstract Augmentation Bake(GameObject player);

    #endregion
}