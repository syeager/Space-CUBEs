// Steve Yeager
// 12.18.2014

using System;
using UnityEngine;

public class ActivateButton : UIButton
{
    public string value;
    public EventHandler<ActivateButtonArgs> ActivateEvent;


    protected override void OnPress(bool isPressed)
    {
        if (ActivateEvent != null)
        {
            ActivateEvent(this, new ActivateButtonArgs(value, isPressed));
        }
    }
}