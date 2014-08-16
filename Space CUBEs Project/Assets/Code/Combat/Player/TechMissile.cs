// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.16
// Edited: 2014.08.16

public class TechMissile : Hitbox, IEMPBlastListener
{
    #region IEMPBlastListener

    public void InteractEMP()
    {
        myPoolObject.Disable();
    }

    #endregion
}