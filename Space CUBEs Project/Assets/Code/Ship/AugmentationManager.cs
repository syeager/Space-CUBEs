// Little Byte Games
// Author: Steve Yeager
// Created: 2014.04.23
// Edited: 2014.09.08

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Manages initializing and holding all augmentations.
    /// </summary>
    public class AugmentationManager : MonoBehaviour
    {
        #region Public Fields

        /// <summary>List of all augmentations.</summary>
        public Augmentation[] augmentations;

        #endregion

        #region Public Methods

        /// <summary>
        /// Add all augmentations.
        /// </summary>
        /// <param name="augmentationList">Augmentations to save.</param>
        public void Bake(List<Augmentation> augmentationList)
        {
            augmentations = new Augmentation[BuildStats.ExpansionLimit];
            int augmentationLimit = BuildStats.GetAugmentationExpansion();

            foreach (Augmentation augmentation in augmentationList.Where(augmentation => augmentation.index < augmentationLimit))
            {
                augmentations[augmentation.index] = augmentation;
            }
        }


        /// <summary>
        /// Initialize all augmentations.
        /// </summary>
        /// <param name="player">Player to pass to each augmentation.</param>
        public void Initialize(Player player)
        {
            foreach (Augmentation augmentation in augmentations.Where(augmentation => augmentation != null))
            {
                augmentation.Initialize(player);
            }
        }

        #endregion
    }
}