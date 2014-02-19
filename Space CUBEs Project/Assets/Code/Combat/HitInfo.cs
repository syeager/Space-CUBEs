// Steve Yeager
// 12.8.2013

using System;

/// <summary>
/// Info to pass from Hitbox to Health.
/// </summary>
[Serializable]
public class HitInfo
{
    #region Public Fields

    /// <summary>Damage/hit or damage/second.</summary>
    public float damage;

    #endregion

    #region Properties

    public static HitInfo Empty { get { return new HitInfo(); } }

    #endregion


    #region Public Methods

    public HitInfo MultiplyDamage(float multiplier)
    {
        return new HitInfo {damage = this.damage*multiplier};
    }

    #endregion
}