// Steve Yeager
// 3.28.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class MovingShield : Weapon
{
    #region Public Fields
    
    public float amp = 1f;
    public float speed;
    
    #endregion

    #region Private Fields

    private Vector3 start;

    #endregion


    #region Weapon Overrides

    public override void Initialize(Ship sender)
    {
        base.Initialize(sender);
        start = myTransform.localPosition - new Vector3(amp * speed, 0f, 0f);
    }


    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            // reset position
            myTransform.localPosition = start;

            gameObject.SetActive(true);
            StartCoroutine(Fire());
        }
        else
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            myTransform.localPosition += new Vector3(amp * Mathf.Sin(timer) * speed * Time.deltaTime, 0f, 0f);
            yield return null;
        }
    }

    #endregion
}