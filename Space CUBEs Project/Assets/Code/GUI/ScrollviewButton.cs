// Steve Yeager
// 2.8.2014

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ScrollviewButton : ActivateButton
{
    #region Public Fields

    public UIDragScrollView dragScrollView;

    #endregion


    #region Public Methods

    public void Initialize(string name, string label, string value, Transform parent, UIScrollView scrollView = null)
    {
        this.name = name;
        this.label.text = label;
        this.value = value;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    #endregion
}