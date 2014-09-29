// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.12
// Edited: 2014.09.15

using System;
using Annotations;
using UnityEngine;

/// <summary>
/// Button that sends out event when pressed/released.
/// </summary>
public class ActivateButton : UIButton
{
    #region Public Fields

    public UILabel label;
    public string value;
    public EventHandler<ActivateButtonArgs> ActivateEvent;

    #endregion

    #region Private Fields

    private bool activated;
    private Color normalColor;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        normalColor = defaultColor;
    }

    #endregion

    #region UIButton Overrides

    protected override void OnPress(bool isPressed)
    {
        base.OnPress(isPressed);
        if (ActivateEvent != null)
        {
            ActivateEvent(this, new ActivateButtonArgs(value, isPressed));
        }
    }


    protected override void OnHover(bool isOver)
    {
        if (activated) return;

        base.OnHover(isOver);
    }


    protected override void OnSelect(bool isSelected)
    {
        if (activated) return;

        base.OnSelect(isSelected);
    }

    #endregion

    #region Public Methods

    public void Initialize(string label, string value)
    {
        this.label.text = label;
        this.value = value;
    }


    public void Activate(bool activate)
    {
        activated = activate;

        defaultColor = activated ? hover : normalColor;
    }


    public void SetText(string text)
    {
        label.text = text;
    }


    public void Toggle(bool on)
    {
        isEnabled = on;
    }

    #endregion
}