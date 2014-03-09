// Steve Yeager
// 1.7.2014

using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour
{
    #region References

    public Transform Center;
    new public Camera camera;
    private Collider myCollider;

    #endregion

    #region Public Fields

    public float maxDistance = 50f;
    public float deadzone = 0.3f;

    #endregion

    #region Private Fields

    private int touchID = -1;
    private Vector2 center;
    private RaycastHit rayInfo;
    private bool cache;

    #endregion

    #region Properties

    public Vector2 value { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        center = GetComponent<UITexture>().CalculateBounds().extents;
      
        myCollider = collider;
    }


    private void Update()
    {
        if (touchID == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                //if (touch.phase == TouchPhase.Began)
                {
                    Physics.Raycast(camera.ScreenPointToRay(touch.position), out rayInfo);
                    if (rayInfo.collider == myCollider)
                    {
                        cache = false;
                        touchID = touch.fingerId;
                        return;
                    }
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
                    Vector2 input = Vector2.ClampMagnitude((touch.position - center) / maxDistance, 1);
                    value = input.magnitude >= deadzone ? input : Vector2.zero;

                    // let go
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        //Debugger.LogConsoleLine(touch.position.ToString(), 1.5f);
                        cache = touch.position.x <= 0f || touch.position.y <= 1f;
                    }

                    return;
                }
            }

            // let go
            touchID = -1;
            if (!cache)
            {
                value = Vector2.zero;
            }
        }
    }

    #endregion
}
