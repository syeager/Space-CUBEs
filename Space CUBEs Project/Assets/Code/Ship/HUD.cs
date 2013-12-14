// Steve Yeager
// 12.8.2013

using UnityEngine;

/// <summary>
/// Player HUD in level.
/// </summary>
public class HUD : Singleton<HUD>
{
    #region References

    private ShieldHealth PlayerHealth;

    #endregion

    #region Static Fields

    public static float NavButton { get; private set; }

    #endregion


    public PressButton[] WeaponButtons = new PressButton[6];
    public PressButton LeftButton;
    public PressButton RightButton;


    #region MonoBehaviour Overrides

    private void Start()
    {
        LeftButton.ActivateEvent += OnActivateNav;
        RightButton.ActivateEvent += OnActivateNav;
    }

    #endregion

    #region Event Handlers

    private void OnActivateNav(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed)
        {
            NavButton += float.Parse(args.value);
        }
        else
        {
            NavButton -= float.Parse(args.value);
        }
    }

    #endregion
}