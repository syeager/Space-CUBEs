// Little Byte Games
// Author: Steve Yeager
// Created: 2014.04.06
// Edited: 2014.10.02

using System;
using System.Collections;
using System.Collections.Generic;
using LittleByte.Extensions;
using SpaceCUBEs;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Builds the player's ship from parts overtime.
/// </summary>
public static class ShowBuild
{
    #region Public Fields

    [Obsolete("Pass into methods.")]
    public static Material material;

    #endregion

    #region Public Methods

    /// <summary>
    /// The ship is built by having all of its parts fly together.
    /// </summary>
    /// <param name="buildInfo">Ship instructions.</param>
    /// <param name="buildSize">Size of the grid.</param>
    /// <param name="transform">Ship transforms.</param>
    /// <param name="maxTime">How long the building process can take.</param>
    /// <param name="finishedAction">Method to call when completed.</param>
    public static IEnumerator Join(BuildInfo buildInfo, int buildSize, Transform transform, float maxTime, Action<BuildFinishedArgs> finishedAction = null)
    {
        var pieces = new List<BuildCUBE>();

        const float minDist = 100f;
        const float maxDist = 250f;
        Vector3 halfGrid = Vector3.one * (buildSize / 2f - 1f);
        Vector3 pivotOffset = -Vector3.one / 2f;
        float speed = maxDist / maxTime;

        foreach (var piece in buildInfo.partList)
        {
            CUBE cube = GameResources.CreateCUBE(piece.Key);
            cube.transform.parent = transform;
            Material[] materials = new Material[cube.renderer.materials.Length];
            materials.SetAll(material);
            cube.renderer.materials = materials;

            cube.transform.localPosition = piece.Value.position.normalized * Random.Range(minDist, maxDist);
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

        if (finishedAction != null)
        {
            finishedAction(new BuildFinishedArgs(transform.gameObject, buildInfo.stats.health, buildInfo.stats.shield, buildInfo.stats.speed, buildInfo.stats.damage));
        }
    }


    public static IEnumerator Disjoin(Transform transform, float time, Action finishedAction = null)
    {
        var pieces = new List<BuildCUBE>();
        const float maxDist = 250f;
        float speed = maxDist / time;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            pieces.Add(new BuildCUBE(child, Utility.RotateVector(Vector3.forward * maxDist, Random.rotation), speed));
        }

        while (time > 0f)
        {
            foreach (BuildCUBE piece in pieces)
            {
                piece.Update(Time.deltaTime);
            }
            time -= Time.deltaTime;
            yield return null;
        }

        if (finishedAction != null)
        {
            finishedAction();
        }
    }

    #endregion
}