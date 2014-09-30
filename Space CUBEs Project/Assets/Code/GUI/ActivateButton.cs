// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.12
// Edited: 2014.09.29

using System;

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

    #region UIButton Overrides

    protected override void OnPress(bool isPressed)
    {
        base.OnPress(isPressed);
        if (ActivateEvent != null)
        {
            ActivateEvent(this, new ActivateButtonArgs(value, isPressed));
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(string label, string value)
    {
        this.label.text = label;
        this.value = value;
    }


    public void Activate(bool activate, bool instant = true)
    {
        enabled = !activate;
        SetState(activate ? State.Pressed : State.Normal, instant);
        if (instant)
        {
            ResetDefaultColor();
        }
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