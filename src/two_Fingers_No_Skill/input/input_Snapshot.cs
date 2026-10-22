namespace two_Fingers_No_Skill.input;

/// <summary>
/// The player actions triggered during one frame ("just pressed", not "held").
/// </summary>
/// <param name="left_Strike_Pressed">Left mouse button (M1) was just pressed.</param>
/// <param name="right_Strike_Pressed">Right mouse button (M2) was just pressed.</param>
/// <param name="confirm_Pressed">Enter was just pressed.</param>
/// <param name="debug_Toggle_Pressed">F1 was just pressed.</param>
/// <param name="pause_Pressed">Space was just pressed.</param>
public readonly record struct input_Snapshot(
    bool left_Strike_Pressed,
    bool right_Strike_Pressed,
    bool confirm_Pressed,
    bool debug_Toggle_Pressed,
    bool pause_Pressed = false)
{
    /// <summary>A frame with no action (used while the window is not focused).</summary>
    public static input_Snapshot empty => default;
}
