// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.15
// Edited: 2014.05.31

using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Empty base class for augmentation CUBEs.
    /// </summary>
    public abstract class Augmentation : MonoBehaviour
    {
        #region Properties

        public int index { get; set; }

        #endregion

        #region Abstract Methods

        public abstract void Initialize(Player player);

        public abstract Augmentation Bake(GameObject player);

        #endregion
    } 
}