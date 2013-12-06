// Steve Yeager
// 12.5.2013

using UnityEngine;

/// <summary>
/// Health manager for all objects that can be destroyed.
/// </summary>
public class Health : MonoBase
{
    #region Properties

    public float maxHealth { get; protected set; }
    public float health { get; protected set; }

    #endregion

    #region Public Methods

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
    }

    #endregion
}