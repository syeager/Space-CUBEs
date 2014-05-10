// Steve Yeager
// 4.28.2014

using UnityEngine;

/// <summary>
/// Restore shield/health after a kill.
/// </summary>
public class VampireDrive : Augmentation
{
    #region Public Fields

    /// <summary>Percentage of health retained from the enemy killed.</summary>
    public float absorbtion = 0.10f;

    #endregion

    #region Private Fields

    /// <summary>Player's health component.</summary>
    private ShieldHealth health;

    #endregion


    #region Augmentation Overrides

    public override void Initialize(Player player)
    {
        health = player.myHealth;
        player.KillRecievedEvent += OnKillReceived;
    }


    public override Augmentation Bake(GameObject player)
    {
        VampireDrive comp = player.AddComponent(typeof (VampireDrive)) as VampireDrive;
        comp.absorbtion = absorbtion;

        return comp;
    }

    #endregion

    #region Event Handlers

    public void OnKillReceived(object sender, KillRecievedArgs args)
    {
        health.ChangeHealth(args.enemyHealthMax * absorbtion);
    }

    #endregion
}