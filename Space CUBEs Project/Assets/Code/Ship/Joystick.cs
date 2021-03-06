﻿// Steve Yeager
// 1.7.2014

using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour
{
    #region References

    new public Camera camera;
    private Collider myCollider;
    public UISprite thumb;

    #endregion

    #region Public Fields

    public float maxDistance = 50f;
    public float deadzone = 0.3f;
    public float xBuffer = 20f;
    public float yBuffer = 20f;

    #endregion

    #region Private Fields

    private int touchID = -1;
    private Vector2 centerScreen;
    private RaycastHit rayInfo;
    private bool cache;
    private Vector2 position;

    #endregion

    #region Properties

    public Vector2 value { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        myCollider = collider;
        
        centerScreen = GetComponent<UIWidget>().CalculateBounds().extents;

        thumb.leftAnchor.target = null;
        thumb.rightAnchor.target = null;
        thumb.topAnchor.target = null;
        thumb.bottomAnchor.target = null;

        // settings
        maxDistance *= (1f - GameSettings.Main.joystickSensitivity);
        xBuffer *= GameSettings.Main.joystickXBuffer;
        yBuffer *= GameSettings.Main.joystickYBuffer;
        deadzone = GameSettings.Main.joystickDeadzone;
    }


    private void Update()
    {
        if (touchID == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Physics.Raycast(camera.ScreenPointToRay(touch.position), out rayInfo);
                if (rayInfo.collider == myCollider)
                {
                    cache = false;
                    touchID = touch.fingerId;
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId == touchID)
                {
                    // apply deadzone
                    position = touch.position;
                    Vector2 input = Vector2.ClampMagnitude((position - centerScreen) / maxDistance, 1);
                    value = input.magnitude >= deadzone ? input : Vector2.zero;
                    // let go
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        cache = touch.position.x <= xBuffer || touch.position.y <= yBuffer;
                    }

                    MoveJoystick();

                    return;
                }
            }

            // let go
            touchID = -1;
            if (!cache)
            {
                position = Vector2.zero;
                value = Vector2.zero;
                MoveJoystick();
            }
        }
    }

    #endregion

    #region Private Methods

    private void MoveJoystick()
    {
        // move joystick
        thumb.transform.localPosition = new Vector3(value.x*maxDistance, value.y* maxDistance, 0);
    }

    #endregion
}
