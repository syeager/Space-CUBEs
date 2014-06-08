// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.06
// Edited: 2014.05.30

using System;
using System.Collections;
using System.Collections.Generic;
using GameSaveData;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Builds the player's ship from parts overtime.
/// </summary>
public class ShowBuild : MonoBehaviour
{
    #region Public Methods

    /// <summary>
    /// The ship is built by having all of its parts fly together.
    /// </summary>
    /// <param name="buildInfo">Ship instructions.</param>
    /// <param name="buildSize">Size of the grid.</param>
    /// <param name="startPosition">Ship's position.</param>
    /// <param name="startRotation">Ship's rotation.</param>
    /// <param name="maxTime">How long the building process can take.</param>
    /// <param name="finishedAction">Method to call when completed.</param>
    public IEnumerator Build(BuildInfo buildInfo, int buildSize, Vector3 startPosition, Vector3 startRotation, float maxTime, Action<BuildFinishedArgs> finishedAction)
    {
        var pieces = new List<BuildCUBE>();

        const float minDist = 100f;
        const float maxDist = 250f;
        Vector3 halfGrid = Vector3.one * (buildSize / 2f - 1f);
        var pivotOffset = new Vector3(-0.5f, -0.5f, -0.5f);
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        float speed = maxDist / maxTime;

        foreach (var piece in buildInfo.partList)
        {
            var cube = (CUBE)Instantiate(GameResources.GetCUBE(piece.Key));
            cube.transform.parent = transform;

            cube.transform.localPosition = piece.Value.position.Cast().normalized * Random.Range(minDist, maxDist);
            cube.transform.localPosition = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)) * cube.transform.localPosition;
            cube.transform.localEulerAngles = piece.Value.rotation;

            if (piece.Value.weaponMap != -1)
            {
                ((Weapon)cube.GetComponent(typeof(Weapon))).index = piece.Value.weaponMap;
            }

            if (piece.Value.augmentationMap != -1)
            {
                ((Augmentation)cube.GetComponent(typeof(Augmentation))).index = piece.Value.augmentationMap;
            }

            ((ColorVertices)cube.GetComponent(typeof(ColorVertices))).Bake(piece.Value.colors);

            pieces.Add(new BuildCUBE(cube.transform, piece.Value.position - halfGrid + Utility.RotateVector(pivotOffset, Quaternion.Euler(piece.Value.rotation)), speed));
        }

        float time = maxTime;
        while (time > 0f)
        {
            foreach (BuildCUBE piece in pieces)
            {
                piece.Update(Time.deltaTime);
            }
            time -= Time.deltaTime;
            yield return null;
        }

        finishedAction(new BuildFinishedArgs(gameObject, buildInfo.health, buildInfo.shield, buildInfo.speed, buildInfo.damage));
        Destroy(this);
    }

    #endregion
}