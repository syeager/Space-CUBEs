// Steve Yeager
// 5.15.2014

using UnityEngine;

/// <summary>
/// Override to stop startOnSelected for Mobile.
/// </summary>
public class KeyNav : UIKeyNavigation
{
    #region MonoBehaviour Overrides

    protected override void OnEnable()
    {
        list.Add(this);

#if UNITY_STANDALONE
        if (startsSelected)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (UICamera.selectedObject == null || !NGUITools.GetActive(UICamera.selectedObject))
            {
                UICamera.currentScheme = UICamera.ControlScheme.Controller;
                UICamera.selectedObject = gameObject;
            }
        }
#endif
    }

    #endregion
}