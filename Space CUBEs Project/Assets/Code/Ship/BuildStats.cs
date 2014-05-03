// Steve Yeager
// 4.28.2014

using UnityEngine;

/// <summary>
/// Holds info for the player's current ship construction stats.
/// </summary>
public static class BuildStats
{
    #region Readonly Fields

    public static readonly int[] CoreCapacities = { 100, 200, 300, 400, 500 };
    public static readonly int[] WeaponExpansions = { 1, 2, 3, 4 };
    public static readonly int[] AugmentationExpansions = { 1, 2, 3, 4 };

    #endregion

    #region Const Fields

    private const string CoreCapacityPath = "Core Capacity";
    private const string WeaponExpansionPath = "Weapon Expansion";
    private const string AugmentationExpansionPath = "Augmentation Expansion";

    #endregion


    #region Public Methods

    public static int GetCoreCapacity()
    {
        return CoreCapacities[PlayerPrefs.GetInt(CoreCapacityPath)];
    }


    public static void SetCoreCapacity(int coreCapacity)
    {
        PlayerPrefs.SetInt(CoreCapacityPath, coreCapacity);
    }


    public static int GetWeaponExpansion()
    {
        return WeaponExpansions[PlayerPrefs.GetInt(WeaponExpansionPath)];
    }


    public static void SetWeaponExpansion(int weaponExpansion)
    {
        PlayerPrefs.SetInt(WeaponExpansionPath, weaponExpansion);
    }


    public static int GetAugmentationExpansion()
    {
        return AugmentationExpansions[PlayerPrefs.GetInt(AugmentationExpansionPath)];
    }


    public static void SetAugmentationExpansion(int augmentationExpansion)
    {
        PlayerPrefs.SetInt(AugmentationExpansionPath, augmentationExpansion);
    }
    
    #endregion
}