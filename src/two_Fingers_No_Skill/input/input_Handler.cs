using Microsoft.Xna.Framework.Input;

namespace two_Fingers_No_Skill.input;

/// <summary>
/// Reads the mouse and keyboard and turns them into "just pressed" actions.
/// </summary>
public sealed class input_Handler
{
    private MouseState previous_Mouse = Mouse.GetState();
    private KeyboardState previous_Keyboard = Keyboard.GetState();

    /// <summary>
    /// Reads the devices and returns the actions started since the previous call.
    /// Must be called once per frame, even when the result is ignored, to keep the edge detection right.
    /// </summary>
    public input_Snapshot read()
    {
        var current_Mouse = Mouse.GetState();
        var current_Keyboard = Keyboard.GetState();

        var snapshot = new input_Snapshot(
            left_Strike_Pressed: current_Mouse.LeftButton == ButtonState.Pressed && previous_Mouse.LeftButton == ButtonState.Released,
            right_Strike_Pressed: current_Mouse.RightButton == ButtonState.Pressed && previous_Mouse.RightButton == ButtonState.Released,
            confirm_Pressed: is_Just_Pressed(current_Keyboard, Keys.Enter),
            debug_Toggle_Pressed: is_Just_Pressed(current_Keyboard, Keys.F1),
            pause_Pressed: is_Just_Pressed(current_Keyboard, Keys.Space));

        previous_Mouse = current_Mouse;
        previous_Keyboard = current_Keyboard;
        return snapshot;
    }

    /// <summary>
    /// True when the key is down now but was up on the previous frame.
    /// </summary>
    private bool is_Just_Pressed(KeyboardState current_Keyboard, Keys key) =>
        current_Keyboard.IsKeyDown(key) && previous_Keyboard.IsKeyUp(key);
}
