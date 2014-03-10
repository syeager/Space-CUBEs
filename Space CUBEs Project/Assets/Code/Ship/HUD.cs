﻿// Steve Yeager
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

    public GameObject multX;
    public UISprite[] multipliers;
    private ShieldHealth PlayerHealth;
    public ActivateButton[] weaponButtons;
    public Joystick joystick;
    public ActivateButton barrelRoll;
    public UISprite ShieldBar;
    public UISprite HealthBar;
    public UILabel Points;

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
        // multiplier
        multX.SetActive(false);
        foreach (var mult in multipliers)
        {
            mult.gameObject.SetActive(false);
        }
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
        for (int i = 0; i < 4; i++)
        {
            Main.weaponButtons[i].GetComponent<WeaponButton>().Disable();
        }
        for (int i = 0; i < player.myWeapons.weapons.Length; i++)
        {
            if (player.myWeapons.weapons[i] != null)
            {
                HUD.Main.weaponButtons[i].ActivateEvent += player.myWeapons.OnActivate;
                Main.weaponButtons[i].GetComponent<WeaponButton>().Initialize(player.myWeapons.weapons[i]);
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
            foreach (var mult in multipliers)
            {
                mult.gameObject.SetActive(false);
            }
        }
        else
        {
            multX.SetActive(true);
            string multiplier = args.multiplier.ToString();
            for (int i = 0; i < multiplier.Length; i++)
            {
                multipliers[i].gameObject.SetActive(true);
                multipliers[i].spriteName = "multiplier"+multiplier[i];
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