// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.28
// Edited: 2014.05.29

using LittleByte.Data;
using UnityEngine;

/// <summary>
/// Holds info for the player's current ship construction stats.
/// </summary>
public static class BuildStats
{
    #region Readonly Fields

    public static readonly int[] CoreCapacities = {200, 400, 800, 1600, 3200};
    public static readonly int[] CorePrices = {0, 1000, 2000, 4000, 8000};
    public static readonly int[] WeaponExpansions = {1, 2, 3, 4};
    public static readonly int[] WeaponPrices = {0, 1000, 2000, 3000};
    public static readonly int[] AugmentationExpansions = {1, 2, 3, 4};
    public static readonly int[] AugmentationPrices = {100, 1000, 2000, 3000};

    #endregion

    #region Const Fields

    private const string ItemsFile = @"Items/";
    private const string CoreCapacityPath = "Core Capacity";
    private const string WeaponExpansionPath = "Weapon Expansion";
    private const string AugmentationExpansionPath = "Augmentation Expansion";

    /// <summary>Total possible weapon and augmentation expansions.</summary>
    public const int ExpansionLimit = 4;

    #endregion

    #region Public Methods

    /// <summary>
    /// Get current core capacity level from data.
    /// </summary>
    /// <returns>0 based index corresponding to core level.</returns>
    public static int GetCoreLevel()
    {
        return SaveData.Load<int>(CoreCapacityPath, ItemsFile);
    }


    /// <summary>
    /// Save the core capacity to data.
    /// </summary>
    /// <param name="coreCapacity">0 based index corresponding to core level.</param>
    public static void SetCoreLevel(int coreCapacity)
    {
        SaveData.Save(CoreCapacityPath, coreCapacity, ItemsFile);
    }


    /// <summary>
    /// Get current core capacity from data.
    /// </summary>
    /// <returns>Core capacity points.</returns>
    public static int GetCoreCapacity()
    {
        return CoreCapacities[GetCoreLevel()];
    }


    /// <summary>
    /// Get current weapon expansion level from data.
    /// </summary>
    /// <returns>0 based index corresponding to weapon level.</returns>
    public static int GetWeaponLevel()
    {
        return SaveData.Load<int>(WeaponExpansionPath, ItemsFile);
    }


    /// <summary>
    /// Save the weapon expansion to data.
    /// </summary>
    /// <param name="weaponExpansion">0 based index corresponding to weapon level.</param>
    public static void SetWeaponLevel(int weaponExpansion)
    {
        SaveData.Save(WeaponExpansionPath, weaponExpansion, ItemsFile);
    }


    /// <summary>
    /// Get current weapon expansion from data.
    /// </summary>
    /// <returns>Weapon expansion points.</returns>
    public static int GetWeaponExpansion()
    {
        return WeaponExpansions[GetWeaponLevel()];
    }


    /// <summary>
    /// Get current augmentation expansion level from data.
    /// </summary>
    /// <returns>0 based index corresponding to augmentation level.</returns>
    public static int GetAugmentationLevel()
    {
        return SaveData.Load<int>(AugmentationExpansionPath, ItemsFile);
    }


    /// <summary>
    /// Save the augmentation expansion to data.
    /// </summary>
    /// <param name="augmentationExpansion">0 based index corresponding to augmentation level.</param>
    public static void SetAugmentationLevel(int augmentationExpansion)
    {
        SaveData.Save(AugmentationExpansionPath, augmentationExpansion, ItemsFile);
    }


    /// <summary>
    /// Get current augmentation expansion from data.
    /// </summary>
    /// <returns>Augmentation expansion points.</returns>
    public static int GetAugmentationExpansion()
    {
        return AugmentationExpansions[GetAugmentationLevel()];
    }

    #endregion
}