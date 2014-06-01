// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.01
// Edited: 2014.06.01

using UnityEngine;

/// <summary>
/// Has special functionality for interacting with Black Holes.
/// </summary>
public interface IBlackHoleListener
{
    #region Methods

    /// <summary>
    /// Enacts special behaviour for interacting with Black Holes.
    /// </summary>
    /// <param name="position">Position of black hole.</param>
    /// <param name="pull">Strength and direction of the Black Hole's pull.</param>
    void Interact(Vector3 position, Vector3 pull);

    #endregion
}