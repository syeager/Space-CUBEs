﻿// Steve Yeager
// 1.23.2014

using UnityEngine;
using System.Collections;
using System;

//
public class WeaponButton : MonoBehaviour
{
    #region Public Fields

    public UISprite cooldownOverlay;
    public UILabel number;

    #endregion

    #region Private Fields

    private Weapon weapon;

    #endregion

    #region Readonly Fields

    private readonly Color readyColor = Color.green;
    private readonly Color cooldownColor = Color.red;
    private readonly Color disabledColor = Color.gray;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        Disable();
    }

    #endregion

    #region Public Methods

    public void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        GetComponent<ActivateButton>().isEnabled = true;
        number.color = readyColor;
        cooldownOverlay.enabled = false;

        weapon.PowerUpdateEvent += OnPowerUpdate;
        weapon.ActivatedEvent += OnActivated;
    }


    public void Disable()
    {
        GetComponent<ActivateButton>().isEnabled = false;
        number.color = disabledColor;
        cooldownOverlay.enabled = false;
    }

    #endregion

    #region Event Handlers

    private void OnPowerUpdate(object sender, ValueArgs args)
    {
        // full power
        if ((float)args.value >= Weapon.FULLPOWER)
        {
            number.color = readyColor;
            cooldownOverlay.enabled = false;
        }
        else
        {
            cooldownOverlay.fillAmount = 1-(float)args.value / Weapon.FULLPOWER;
        }
    }


    private void OnActivated(object sender, EventArgs args)
    {
        number.color = cooldownColor;
        cooldownOverlay.enabled = true;
        cooldownOverlay.fillAmount = 1f;
    }

    #endregion
}