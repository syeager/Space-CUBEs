﻿// Steve Yeager
// 11.25.2013

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player ship controller.
/// </summary>
public class Player : Ship
{
    #region State Fields

    private const string MovingState = "Moving";

    #endregion

    #region Input Fields

    public float horizontalBounds = 0.05f;
    public float backBounds;
    public float frontBounds;
    public readonly KeyCode[] WeaponKeys = new KeyCode[6]
    {
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.Semicolon,
        KeyCode.U,
        KeyCode.I
    };

    #endregion


    #region Unity Overrides

    protected override void Awake()
    {
        base.Awake();

        // setup
        tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");

        // create states
        stateMachine.CreateState(MovingState, MovingEnter, MovingExit);
        stateMachine.CreateState(DyingState, DyingEnter, info => { });
        stateMachine.initialState = MovingState;
    }


    protected override void Start()
    {
        base.Start();

        HUD.Initialize(this);
        myWeapons.RegisterToHUD();

        stateMachine.Start(null);
    }

    #endregion

    #region State Methods

    private void MovingEnter(Dictionary<string, object> info)
    {
        StartCoroutine("MovingUpdate");
    }


    private IEnumerator MovingUpdate()
    {
        while (true)
        {
            // attack
            var weapons = AttackInput();
            if (weapons.Count > 0)
            {
                //SetState(AttackingState, new Dictionary<string, object> { { "weapons", weapons } });
                //yield break;
                Attack(weapons);
            }

            // move
            myMotor.Move(HorizontalInput(), Vector3.right);

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        stateMachine.update = new Job(MovingUpdate());
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        Destroy(gameObject);
    }

    #endregion

    #region Input Methods

    private float HorizontalInput()
    {
        float input = 0f;
        float ypos = Camera.main.WorldToViewportPoint(myTransform.position).y;

        #if UNITY_STANDALONE

        input = Input.GetAxis("Horizontal");

        #else

        input = HUD.NavButton;

        #endif

        // down
        if (input > 0f && ypos <= horizontalBounds)
        {
            return 0f;
        }
        // up
        else if (input < 0f && ypos >= 1 - horizontalBounds)
        {
            return 0f;
        }
        else
        {
            return input;
        }
    }


    private List<KeyValuePair<int, bool>> AttackInput()
    {
        var weapons = new List<KeyValuePair<int, bool>>();

        #if UNITY_STANDALONE

        for (int i = 0; i < WeaponKeys.Length; i++)
        {
            if (myWeapons.CanActivate(i))
            {
                if (Input.GetKeyDown(WeaponKeys[i]))
                {
                    weapons.Add(new KeyValuePair<int, bool>(i, true));
                }
                else if (Input.GetKeyUp(WeaponKeys[i]))
                {
                    weapons.Add(new KeyValuePair<int, bool>(i, false));
                }
            }
        }

        #endif

        return weapons;
    }

    #endregion

    #region Combat Methods

    private void Attack(List<KeyValuePair<int, bool>> weapons)
    {
        foreach (var weapon in weapons)
        {
            myWeapons.Activate(weapon.Key, weapon.Value);
        }
    }

    #endregion

    #region Private Methods

    private void CombineMesh()
    {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        var meshes = new CombineInstance[meshFilters.Length]; 
        for (int i = 0; i < meshFilters.Length; i++)
        {
            meshes[i].mesh = meshFilters[i].sharedMesh;
            meshes[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(meshes);
    }

    #endregion
}