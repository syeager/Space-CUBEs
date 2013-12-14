// Steve Yeager
// 

using System;

public class ActivateButtonArgs : EventArgs
{
    public readonly string value;
    public readonly bool isPressed;


    public ActivateButtonArgs(string value, bool isPressed)
    {
        this.value = value;
        this.isPressed = isPressed;
    }
}