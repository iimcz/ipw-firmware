using UnityEngine;

public static class InputExtensions
{
    /// <summary>
    /// Return either continuous or single keypresses depending on whether right shift is being held.
    /// </summary>
    /// <param name="code">Checked keycode</param>
    /// <returns>Whether the key is being pressed given the modifier</returns>
    public static bool GetKeyModified(KeyCode code)
    {
        return Input.GetKey(KeyCode.RightShift) ? Input.GetKeyDown(code) : Input.GetKey(code);
    }
}