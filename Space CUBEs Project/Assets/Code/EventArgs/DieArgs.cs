// Little Byte Games

using System;

namespace SpaceCUBEs
{
    // Steve Yeager
    // 12.16.2013
    
    public class DieArgs : EventArgs
    {
        public readonly Ship killer;

        public DieArgs(Ship killer)
        {
            this.killer = killer;
        }
    }
}