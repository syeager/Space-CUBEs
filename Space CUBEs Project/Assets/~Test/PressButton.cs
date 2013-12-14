// Steve Yeager
// 

using System;
using UnityEngine;

public class PressButton : UIButton
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