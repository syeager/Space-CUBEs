// Little Byte Games
// Author: Steve Yeager
// Created: 2014.02.08
// Edited: 2014.09.14

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

    public void Initialize(string name, string label, string value, Transform parent, UIScrollView scrollView)
    {
        this.name = name;
        this.label.text = label;
        this.value = value;
        dragScrollView.scrollView = scrollView;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    #endregion
}