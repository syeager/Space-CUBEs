// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.14
// Edited: 2014.08.14

using UnityEngine;

public class LaserLockLaser : Hitbox
{
    #region References

    public LaserLock laserLock;

    #endregion

    #region MonoBehaviour Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        var oppHealth = other.gameObject.GetComponent(typeof(Health)) as Health;
        if (oppHealth != null)
        {
            // send damage
            oppHealth.RecieveHit(sender, damage);

            // player
            if (other.CompareTag(Player.Tag))
            {
                laserLock.Freeze(true);
            }

            disabled = true;
            myPoolObject.Disable();
        }
    }

    #endregion
}