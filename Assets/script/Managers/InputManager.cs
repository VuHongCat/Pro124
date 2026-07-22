using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool LeftMouse => Input.GetMouseButtonDown(0);
    public bool RightMouse => Input.GetMouseButtonDown(1);
    public bool Escape => Input.GetKeyDown(KeyCode.Escape);
}
