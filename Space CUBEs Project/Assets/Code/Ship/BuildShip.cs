using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class BuildShip : MonoBehaviour
{
    public void Build()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 1; i < meshFilters.Length; i++)
        {

            //meshFilters[i].mesh.CombineMeshes(new CombineInstance[0]);




            combine[i].mesh = meshFilters[i].sharedMesh;
            var child = meshFilters[i].transform;
            combine[i].transform = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale);
            Destroy(meshFilters[i].gameObject);
            //meshFilters[i].gameObject.SetActive(false);
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().mesh.Optimize();
        transform.renderer.sharedMaterial = GameResources.Main.VertexColor_Mat;
    }
}
