// Steve Yeager
// 11.25.2013

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all ships.
/// </summary>
[RequireComponent(typeof(ShipMotor))]
[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(ShieldHealth))]
public class Ship : MonoBase
{
    #region References

    protected Transform myTransform;
    protected ShipMotor myMotor;
    public WeaponManager myWeapons;
    protected ShieldHealth myHealth;

    #endregion

    #region Protected Fields

    protected StateMachine stateMachine;

    #endregion

    #region Const Fields

    protected const string SpawningState = "Spawning";
    protected const string DyingState = "Dying";

    private const float COLLISIONDAMAGE = -20f;

    #endregion


    #region Unity Overrides

    protected virtual void Awake()
    {
        // get references
        myTransform = transform;
        myMotor = GetComponent<ShipMotor>() ?? gameObject.AddComponent<ShipMotor>();
        myWeapons = GetComponent<WeaponManager>() ?? gameObject.AddComponent<WeaponManager>();
        myHealth = GetComponent<ShieldHealth>() ?? gameObject.AddComponent<ShieldHealth>();

        stateMachine = new StateMachine(this);
    }

    protected virtual void Start()
    {
        // combat
        myWeapons.Initialize(this);

        // register events
        myHealth.DieEvent += OnDie;
    }


    private void OnTriggerEnter(Collider other)
    {
        Health otherHealth = other.GetComponent<Health>();
        if (otherHealth != null)
        {
            otherHealth.RecieveHit(this, new HitInfo { damage = COLLISIONDAMAGE });
        }
    }


    private void OnDestroy()
    {
        stateMachine = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a collider for the ship that encompasses all CUBEs.
    /// </summary>
    [Obsolete("Will need to rewrite after combining meshes.")]
    public void GenerateCollider()
    {
        Bounds bounds = new Bounds();
        bounds.center = myTransform.position;

        var children = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var child in children)
        {
            bounds.Encapsulate(child.bounds);
        }

        var boxCol = gameObject.AddComponent<BoxCollider>();
        boxCol.isTrigger = true;
        boxCol.size = new Vector3(bounds.size.y, 10f, bounds.size.x);
        boxCol.center = new Vector3(0f, 0, -0.5f); // z is wrong
    }


    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 1; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            var child = meshFilters[i].transform;
            combine[i].transform = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale);

            // get weapon


            Destroy(meshFilters[i].gameObject);
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().mesh.Optimize();
        transform.renderer.sharedMaterial = GameResources.Main.VertexColor_Mat;
    }

    #endregion

    #region Event Handlers

    private void OnDie(object sender, DieArgs args)
    {
        if (stateMachine.currentState != DyingState)
        {
            stateMachine.SetState(DyingState, new Dictionary<string, object> { { "sender", sender } });
        }
    }

    #endregion
}