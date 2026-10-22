namespace two_Fingers_No_Skill.input;

/// <summary>
/// Makes sure the click used to bring the game window back to the front never counts as a strike.
/// The focus event and the mouse press come from the same click, so checking "is the window active"
/// on the same frame is not enough: mouse input stays blocked until every mouse button has been released.
/// </summary>
public sealed class focus_Input_Guard
{
    // Starts blocked so that a click used to focus the window right at launch is ignored too.
    private bool is_Mouse_Blocked = true;

    /// <summary>
    /// Returns the actions the game may use this frame.
    /// </summary>
    /// <param name="raw_Input">Actions read from the devices.</param>
    /// <param name="is_Window_Active">True when the game window has the focus.</param>
    /// <param name="is_Any_Mouse_Button_Held">True when a mouse button is currently down.</param>
    public input_Snapshot filter(input_Snapshot raw_Input, bool is_Window_Active, bool is_Any_Mouse_Button_Held)
    {
        if (!is_Window_Active)
        {
            is_Mouse_Blocked = true;
            return input_Snapshot.empty;
        }

        if (is_Mouse_Blocked && !is_Any_Mouse_Button_Held)
        {
            is_Mouse_Blocked = false;
        }

        // The keyboard is not affected: focusing the window with a click never presses a key.
        return is_Mouse_Blocked
            ? raw_Input with { left_Strike_Pressed = false, right_Strike_Pressed = false }
            : raw_Input;
    }
}
