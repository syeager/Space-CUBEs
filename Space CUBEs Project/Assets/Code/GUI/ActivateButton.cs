// Steve Yeager
// 12.18.2014

using System;
using UnityEngine;

public class ActivateButton : UIButton
{
    public Color activatedColor;
    public UILabel label;
    public string value;
    public EventHandler<ActivateButtonArgs> ActivateEvent;


    protected override void OnPress(bool isPressed)
    {
        if (ActivateEvent != null)
        {
            ActivateEvent(this, new ActivateButtonArgs(value, isPressed));
        }
    }


    public void Activate(bool activate)
    {
        Color c = activate ? activatedColor : defaultColor;
        TweenColor tc = TweenColor.Begin(tweenTarget, 0.15f, c);
        tc.value = c;
        tc.enabled = false;
    }
}