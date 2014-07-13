// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.08
// Edited: 2014.07.12

using System;
using Annotations;
using UnityEngine;

/// <summary>
/// Player HUD in level.
/// </summary>
public class HUD : Singleton<HUD>
{
    #region References

    private Animator animator;

    public GameObject multX;
    public UISprite[] multipliers;
    private ShieldHealth PlayerHealth;
    public ActivateButton[] weaponButtons;
    public Joystick joystick;
    public ActivateButton barrelRoll;
    public UISprite ShieldBar;
    public UISprite HealthBar;
    public UILabel Points;
    public UISprite bossHealth;

    #endregion

    #region Private Fields

    private int killClip;
    private int increaseClip;

    #endregion

    #region Static Fields

    public static float NavButton { get; private set; }

    #endregion

    #region Events

    public EventHandler BarrelRollEvent;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        // references
        animator = GetComponent<Animator>();

        // multiplier
        multX.SetActive(false);
        foreach (UISprite mult in multipliers)
        {
            mult.gameObject.SetActive(false);
        }

        // animations
        killClip = Animator.StringToHash("Kill");
        increaseClip = Animator.StringToHash("Increase");
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

        // barrel roll
        Main.barrelRoll.ActivateEvent += Main.OnBarrelRoll;

        // initialize weapon icons
        for (int i = 0; i < BuildStats.ExpansionLimit; i++)
        {
            ((WeaponButton)Main.weaponButtons[i].GetComponent(typeof(WeaponButton))).Disable();
        }
        for (int i = 0; i < player.Weapons.weapons.Length; i++)
        {
            if (player.Weapons.weapons[i] != null)
            {
                Main.weaponButtons[i].ActivateEvent += player.Weapons.OnActivate;
                ((WeaponButton)Main.weaponButtons[i].GetComponent(typeof(WeaponButton))).Initialize(player.Weapons.weapons[i]);
            }
        }
    }

    #endregion

    #region Event Handlers

    private void OnShieldUpdate(object sender, ShieldUpdateArgs args)
    {
        ShieldBar.fillAmount = (args.shield / args.max);
    }


    private void OnHealthUpdate(object sender, HealthUpdateArgs args)
    {
        HealthBar.fillAmount = (args.health / args.max);
    }


    private void OnPointsChanged(object sender, PointsUpdateArgs args)
    {
        Points.text = String.Format("{0:#,###0}", args.points);
    }


    private void OnMultiplierChanged(object sender, MultiplierUpdateArgs args)
    {
        if (args.multiplier == 1)
        {
            multX.SetActive(false);
            foreach (UISprite mult in multipliers)
            {
                mult.gameObject.SetActive(false);
            }
        }
        else
        {
            animator.Play(args.multiplierGained == 0 ? killClip : increaseClip);

            multX.SetActive(true);
            string multiplier = args.multiplier.ToString();
            for (int i = 0; i < multiplier.Length; i++)
            {
                multipliers[i].gameObject.SetActive(true);
                multipliers[i].spriteName = "multiplier" + multiplier[i];
            }
        }
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