// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.03
// Edited: 2014.10.03

using System;

public class OverlayEventArgs : EventArgs
{
    public readonly string controller;
    public readonly bool activated;

    public static EventHandler<OverlayEventArgs> OverlayEvent;


    public OverlayEventArgs(string controller, bool activated)
    {
        this.controller = controller;
        this.activated = activated;
    }


    public static void Fire(object sender, string controller, bool activated)
    {
        if (OverlayEvent != null)
        {
            OverlayEvent(sender, new OverlayEventArgs(controller, activated));
        }
    }
}