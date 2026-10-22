using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// One screen of the game (title, match, game over...). Adding a screen means adding a class implementing this.
/// </summary>
public interface i_Game_State
{
    /// <summary>
    /// Runs the screen logic for one frame.
    /// </summary>
    /// <param name="real_Delta_Seconds">Real time elapsed since the previous frame.</param>
    /// <param name="input">Actions triggered this frame.</param>
    void update(double real_Delta_Seconds, input_Snapshot input);

    /// <summary>
    /// Draws the screen. The sprite batch is already begun.
    /// </summary>
    void draw(render_Context ctx);
}
