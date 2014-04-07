// Steve Yeager
// 2.22.2014

using UnityEngine;

/// <summary>
/// Button whose color can be freely changed.
/// </summary>
public class ColorButton : ActivateButton
{
    #region Public Fields
    
    public UISprite swatch;
    
    #endregion


    #region Public Methods

    public void SetColor(Color color)
    {
        swatch.color = color;
    }

    #endregion
}