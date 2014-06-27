// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.25
// Edited: 2014.06.25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns minions for Medic.
/// </summary>
public class MinionSpawner : Weapon
{
    #region Public Fields

    public PoolObject minionPrefab;

    #endregion

    #region Weapon Overrides

    public Coroutine Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (pressed)
        {
            return StartCoroutine(Spawn(0, null));
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Spawn(int count, List<Health> minions)
    {
        yield return null;
    }

    #endregion
}