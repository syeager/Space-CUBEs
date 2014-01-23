// Steve Yeager
// 1.22.2014

using System;

public class CashUpdateArgs : EventArgs
{
    public readonly int cash;


    public CashUpdateArgs(int cash)
    {
        this.cash = cash;
    }
}