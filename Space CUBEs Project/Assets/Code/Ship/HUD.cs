// Steve Yeager
// 12.8.2013

using System;
using UnityEngine;

/// <summary>
/// Player HUD in level.
/// </summary>
public class HUD : Singleton<HUD>
{
    #region References

    private ShieldHealth PlayerHealth;
    public PressButton[] WeaponButtons = new PressButton[6];
    public PressButton LeftButton;
    public PressButton RightButton;
    public UITexture ShieldBar;
    public UITexture HealthBar;
    public UILabel Points;
    public UILabel Multiplier;

    #endregion

    #region Private Fields

    public float barPer;

    #endregion

    #region Static Fields

    public static float NavButton { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        LeftButton.ActivateEvent += OnActivateNav;
        RightButton.ActivateEvent += OnActivateNav;

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
    }

    #endregion

    #region Event Handlers

    private void OnActivateNav(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed)
        {
            NavButton += float.Parse(args.value);
        }
        else
        {
            NavButton -= float.Parse(args.value);
        }
    }


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

    #endregion
}