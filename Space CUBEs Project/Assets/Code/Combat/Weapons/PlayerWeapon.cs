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
    public abstract Weapon Bake(GameObject parent);

    #endregion
}