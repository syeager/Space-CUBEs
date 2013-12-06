// Steve Yeager
// 12.5.2013

using UnityEngine;

/// <summary>
/// Health manager for objects that have a shield.
/// </summary>
public class ShieldHealth : Health
{
    #region Properties

    public float maxShield { get; private set; }
    public float shield { get; private set; }

    #endregion


    #region Public Methods

    public void Initialize(float maxHealth, float maxShield)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
        this.maxShield = maxShield;
        shield = maxShield;
    }

    #endregion
}