// Steve Yeager
// 1.23.2014

using System;

//
public class ValueArgs : EventArgs
{
    public readonly object value;


    public ValueArgs(object value)
    {
        this.value = value;
    }
}