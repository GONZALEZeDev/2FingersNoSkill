using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// Holds the active screen and forwards the frame to it.
/// </summary>
public sealed class game_State_Machine
{
    private i_Game_State? active_State;

    /// <summary>The active screen.</summary>
    public i_Game_State current_State =>
        active_State ?? throw new InvalidOperationException("No game state has been set yet.");

    /// <summary>
    /// Switches to another screen. Takes effect immediately; the new screen updates from the next frame.
    /// </summary>
    public void change_State(i_Game_State next_State)
    {
        active_State = next_State;
    }

    /// <summary>
    /// Updates the active screen.
    /// </summary>
    public void update(double real_Delta_Seconds, input_Snapshot input) => current_State.update(real_Delta_Seconds, input);

    /// <summary>
    /// Draws the active screen.
    /// </summary>
    public void draw(render_Context ctx) => current_State.draw(ctx);
}
