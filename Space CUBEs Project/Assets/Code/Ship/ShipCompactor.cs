using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class ShipCompactor : MonoBehaviour
{
    public Ship Compact(Type shipType, bool cleanUnused, params Type[] components)
    {
        List<Weapon> weapons = new List<Weapon>();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();
        for (int i = 1; i < meshFilters.Length; i++)
        {
            // bake weapons. save refs for weapon manager
            var weapon = meshFilters[i].GetComponent<Weapon>();
            if (weapon != null)
            {
                weapons.Add(weapon.Bake(gameObject));
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
        
        if (cleanUnused)
        {
            Resources.UnloadUnusedAssets();
        }

        // add ship essentials
        Ship ship = gameObject.AddComponent(shipType.ToString()) as Ship;
        ship.myWeapons.Bake(weapons);
        var box = ship.gameObject.AddComponent<BoxCollider>();
        box.size = new Vector3(box.size.x, 10f, box.size.z);
        box.isTrigger = true;
        var body = ship.gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;

        // add extra components
        foreach (var comp in components)
        {
            ship.gameObject.AddComponent(comp.ToString());
        }

        Destroy(this);
        return ship;
    }


    //
    public Ship Compact(params Type[] components)
    {
        return Compact(typeof(Ship), false, components);
    }
}
