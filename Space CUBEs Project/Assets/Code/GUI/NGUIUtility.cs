// Little Byte Games

using SpaceCUBEs;

public static class NGUIUtility
{
    public static void SetSelected(UIButton button)
    {
        if (InputManager.ActiveInput == InputManager.Inputs.Touch) return;

        UICamera.selectedObject = button.gameObject;
        button.SetState(UIButtonColor.State.Hover, true);
    }
}