// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.03
// Edited: 2014.10.03

using System;

public class OverlayEventArgs : EventArgs
{
    public readonly string group;
    public readonly bool activated;

    public static EventHandler<OverlayEventArgs> OverlayEvent;


    public OverlayEventArgs(string group, bool activated)
    {
        this.group = group;
        this.activated = activated;
    }


    public static void Fire(object sender, string group, bool activated)
    {
        if (OverlayEvent != null)
        {
            OverlayEvent(sender, new OverlayEventArgs(group, activated));
        }
    }
}