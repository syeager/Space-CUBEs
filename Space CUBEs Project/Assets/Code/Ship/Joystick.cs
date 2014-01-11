// Steve Yeager
// 1.7.2014

using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour
{
    #region References

    new public Camera camera;

    #endregion

    #region Public Fields

    public float maxDistance = 50f;
    public float deadzone = 0.2f;

    #endregion

    #region Private Fields

    private int touchID = -1;
    private Vector2 center;

    #endregion

    #region Properties

    public Vector2 value { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        center = camera.WorldToScreenPoint(transform.position);
    }


    private void Update()
    {
        if (touchID == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    if (Physics.Raycast(camera.ScreenPointToRay(touch.position)))
                    {
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
                if (Input.GetTouch(i).fingerId == touchID)
                {
                    // apply deadzone
                    Vector2 input = Vector2.ClampMagnitude((Input.GetTouch(i).position - center) / maxDistance, 1);
                    value = input.magnitude >= deadzone ? input : Vector2.zero;
                    return;
                }
            }

            // let go
            touchID = -1;
            value = Vector2.zero;
        }
    }

    #endregion
}
