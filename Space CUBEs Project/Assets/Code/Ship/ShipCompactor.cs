// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.19
// Edited: 2014.06.27

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Combines ship mesh and adds appropriate components.
/// </summary>
public class ShipCompactor : MonoBehaviour
{
    #region Const Fields

    private const float PlayerCollider = 0.9f;

    #endregion

    #region Public Methods

    public Ship Compact(bool cleanUnused, params Type[] components)
    {
        // add ship
        var ship = (Player)GetComponent(typeof(Player));

        GameObject myGameObject = gameObject;

        var weapons = new List<Weapon>();
        var augmentations = new List<Augmentation>();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        var combine = new List<CombineInstance>();
        for (int i = 1; i < meshFilters.Length; i++)
        {
            // bake weapons. save refs for weapon manager
            var weapon = (PlayerWeapon)meshFilters[i].GetComponent(typeof(PlayerWeapon));
            if (weapon != null)
            {
                weapon.Initialize(ship);
                weapons.Add(weapon.Bake(myGameObject));
            }

            // bake augmentations. save refs for augmentation manager
            var augmentation = meshFilters[i].GetComponent(typeof(Augmentation)) as Augmentation;
            if (augmentation != null)
            {
                augmentations.Add(augmentation.Bake(myGameObject));
            }

            for (int j = 0; j < meshFilters[i].sharedMesh.subMeshCount; j++)
            {
                Transform child = meshFilters[i].transform;
                var combineInstance = new CombineInstance
                {
                    mesh = meshFilters[i].sharedMesh,
                    subMeshIndex = j,
                    transform = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale)
                };
                combine.Add(combineInstance);
            }

            Destroy(meshFilters[i].gameObject);
        }

        var mesh = new Mesh {name = "Player_Mesh"};
        mesh.CombineMeshes(combine.ToArray());
        mesh.Optimize();

        ((MeshFilter)GetComponent(typeof(MeshFilter))).mesh = mesh;
        transform.renderer.sharedMaterial = GameResources.Main.VertexColor_Mat;

        // center mesh
        Vector3 center = mesh.bounds.center;
        transform.position = renderer.bounds.center;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= center;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        if (cleanUnused)
        {
            Resources.UnloadUnusedAssets();
        }


        // bake weapons
        ship.Weapons.Bake(weapons);
        // bake augmentations
        ship.Augmentations.Bake(augmentations);
        // add collider
        var box = (BoxCollider)ship.gameObject.AddComponent(typeof(BoxCollider));
        box.size = box.size * PlayerCollider;
        box.isTrigger = false;

        // add extra components
        foreach (Type comp in components)
        {
            ship.gameObject.AddComponent(comp.ToString());
        }

        Destroy(this);
        return ship;
    }

    #endregion
}