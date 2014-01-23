// Steve Yeager
// 12.16.2013

using UnityEngine;
using System;
using System.Collections.Generic;

public class DieArgs : EventArgs
{
    public readonly Ship killer;


    public DieArgs(Ship killer)
    {
        this.killer = killer;
    }
}