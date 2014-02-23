// Steve Yeager
// 12.18.2014

using System;
using System.Collections;
using UnityEngine;

public class ActivateButton : UIButton
{
    #region Public Fields

    public Color activatedColor;
    public UILabel label;
    public string value;
    public EventHandler<ActivateButtonArgs> ActivateEvent;

    #endregion
    
    #region Private Fields

    private bool activated;

    #endregion


    #region UIButton Overrides

    protected override void OnPress(bool isPressed)
    {
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

    public void Activate(bool activate)
    {
        activated = activate;

        if (isEnabled)
        {
            TweenColor.Begin(tweenTarget, 0f, activate ? activatedColor : defaultColor);
        }
    }


    public void Activate(Color color)
    {
        activated = true;

        if (isEnabled)
        {
            TweenColor.Begin(tweenTarget, 0f, color);
        }
    }

    #endregion
}