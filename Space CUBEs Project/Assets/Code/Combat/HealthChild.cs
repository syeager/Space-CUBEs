﻿// Steve Yeager
// 3.25.2014

using Annotations;

/// <summary>
/// Send hit information to parent.
/// </summary>
public class HealthChild : Health
{
    #region Public Fields
    
    public Health parent;
    
    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        myTransform = parent.myTransform;
    }


    public override float RecieveHit(Ship sender, float damage)
    {
        return parent.RecieveHit(sender, damage);
    }

    #endregion
}