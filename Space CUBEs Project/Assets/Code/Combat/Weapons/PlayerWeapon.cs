// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.25
// Edited: 2014.06.25

using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class PlayerWeapon : Weapon
{
    #region Abstract Methods

    /// <summary>
    /// attach new Weapon to parent
    //  copy values to parent
    //  delete self
    /// </summary>
    /// <param name="parent">GameObject to attach copy to.</param>
    /// <returns>Copy of component.</returns>
    public abstract PlayerWeapon Bake(GameObject parent);

    #endregion

    #region Weapon Overrides
    
    /// <summary>
    /// Activate weapon with extra info.
    /// </summary>
    /// <param name="pressed">Is the weapon being pressed?</param>
    /// <param name="multiplier">Damage multiplier.</param>
    public virtual Coroutine Activate(bool pressed, float multiplier = 1f)
    {
        return null;
    }

    #endregion
}