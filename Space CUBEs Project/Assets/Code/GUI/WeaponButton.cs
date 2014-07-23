// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.23
// Edited: 2014.07.22

using Annotations;
using UnityEngine;
using System;

/// <summary>
/// 
/// </summary>
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

    [UsedImplicitly]
    private void Awake()
    {
        Disable();
    }

    #endregion

    #region Public Methods

    public void Initialize(Weapon weapon)
    {
        Enable();

        weapon.PowerUpdateEvent += OnPowerUpdate;
        weapon.ActivatedEvent += OnActivated;
        weapon.EnabledEvent += OnEnabled;
    }


    public void Enable()
    {
        GetComponent<ActivateButton>().isEnabled = true;
        number.color = readyColor;
        cooldownOverlay.enabled = true;
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
        if ((float)args.value >= Weapon.FullPower)
        {
            number.color = readyColor;
            cooldownOverlay.enabled = false;
        }
        else
        {
            cooldownOverlay.fillAmount = 1 - (float)args.value / Weapon.FullPower;
        }
    }


    private void OnActivated(object sender, EventArgs args)
    {
        number.color = cooldownColor;
        cooldownOverlay.enabled = true;
        cooldownOverlay.fillAmount = 1f;
    }


    private void OnEnabled(object sender, ValueArgs args)
    {
        if ((bool)args.value)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    #endregion
}