// Steve Yeager
// 4.15.2014
// TODO: Clean and break up.
using UnityEngine;
using System.Collections.Generic;
using System;

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
        GameObject myGameObject = gameObject;

        List<Weapon> weapons = new List<Weapon>();
        List<Augmentation> augmentations = new List<Augmentation>();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();
        for (int i = 1; i < meshFilters.Length; i++)
        {
            // bake weapons. save refs for weapon manager
            var weapon = meshFilters[i].GetComponent<Weapon>();
            if (weapon != null)
            {
                weapons.Add(weapon.Bake(myGameObject));
            }

            // bake augmentations. save refs for augmentation manager
            var augmentation = meshFilters[i].GetComponent<Augmentation>();
            if (augmentation != null)
            {
                augmentations.Add(augmentation.Bake(myGameObject));
            }

            for (int j = 0; j < meshFilters[i].sharedMesh.subMeshCount; j++)
            {
                var child = meshFilters[i].transform;
                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = meshFilters[i].sharedMesh,
                    subMeshIndex = j,
                    transform = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale)
                };
                combine.Add(combineInstance);
            }

            Destroy(meshFilters[i].gameObject);
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
        transform.GetComponent<MeshFilter>().mesh.Optimize();
        transform.renderer.sharedMaterial = GameResources.Main.VertexColor_Mat;

        // center mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
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

        // add ship
        Player ship = GetComponent<Player>();
        // bake weapons
        ship.myWeapons.Bake(weapons);
        // bake augmentations
        ship.myAugmentations.Bake(augmentations);
        // add collider
        var box = (BoxCollider)ship.gameObject.AddComponent(typeof(BoxCollider));
        box.size = box.size * PlayerCollider;
        box.isTrigger = false;

        // add extra components
        foreach (var comp in components)
        {
            ship.gameObject.AddComponent(comp.ToString());
        }

        Destroy(this);
        return ship;
    }

    #endregion
}