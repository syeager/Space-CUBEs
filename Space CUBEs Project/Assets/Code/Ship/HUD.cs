// Steve Yeager
// 12.8.2013

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Player HUD in level.
/// </summary>
public class HUD : Singleton<HUD>
{
    #region References

    private ShieldHealth PlayerHealth;
    public PressButton[] WeaponButtons;
    public Joystick joystick;
    public PressButton barrelRoll;
    public UITexture ShieldBar;
    public UITexture HealthBar;
    public UILabel Points;
    public UILabel Multiplier;

    #endregion

    #region Private Fields

    private float barPer;

    #endregion

    #region Static Fields

    public static float NavButton { get; private set; }

    #endregion

    #region Events

    public EventHandler BarrelRollEvent;

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        barPer = ShieldBar.rightAnchor.relative - ShieldBar.leftAnchor.relative;
    }

    #endregion

    #region Static Methods

    public static void Initialize(Player player)
    {
        Main.PlayerHealth = player.GetComponent<ShieldHealth>();
        Main.PlayerHealth.ShieldUpdateEvent += Main.OnShieldUpdate;
        Main.PlayerHealth.HealthUpdateEvent += Main.OnHealthUpdate;

        player.myScore.PointsUpdateEvent += Main.OnPointsChanged;
        player.myScore.MultiplierUpdateEvent += Main.OnMultiplierChanged;

        Main.barrelRoll.ActivateEvent += Main.OnBarrelRoll;
    }

    #endregion

    #region Event Handlers

    private void OnShieldUpdate(object sender, ShieldUpdateArgs args)
    {
        ShieldBar.rightAnchor.relative = ShieldBar.leftAnchor.relative + barPer * (args.shield / args.max);
    }


    private void OnHealthUpdate(object sender, HealthUpdateArgs args)
    {
        HealthBar.rightAnchor.relative = HealthBar.leftAnchor.relative + barPer * (args.health / args.max);
    }


    private void OnPointsChanged(object sender, PointsUpdateArgs args)
    {
        Points.text = String.Format("P: {0:#,###0}", args.points);
    }


    private void OnMultiplierChanged(object sender, MultiplierUpdateArgs args)
    {
        Multiplier.text = "M: " + args.multiplier.ToString();
    }


    private void OnBarrelRoll(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        if (BarrelRollEvent != null)
        {
            BarrelRollEvent(this, EventArgs.Empty);
        }
    }

    #endregion
}