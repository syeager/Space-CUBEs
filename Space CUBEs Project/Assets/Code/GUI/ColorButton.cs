// Steve Yeager
// 2.22.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class ColorButton : ActivateButton
{
    #region Public Fields
    
    public UISprite swatch;
    
    #endregion

    #region Private Fields

    private Color color;

    #endregion


    #region Public Methods

    public void SetColor(Color color)
    {
        this.color = color;
        swatch.color = color;
    }

    #endregion
}