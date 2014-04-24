// Steve Yeager
// 4.23.2014

using System.Collections.Generic;
using UnityEngine;

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
    /// <param name="augmentations">Augmentations to save.</param>
    public void Bake(List<Augmentation> augmentations)
    {
        this.augmentations = augmentations.ToArray();    
    }


    /// <summary>
    /// Initialize all augmentations.
    /// </summary>
    /// <param name="player">Player to pass to each augmentation.</param>
    public void Initialize(Player player)
    {
        foreach (var augmentation in augmentations)
        {
            if (augmentation != null)
            {
                augmentation.Initialize(player);
            }
        }
    }

    #endregion
}