using Microsoft.Xna.Framework;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// Pause screen: shows the frozen match under a dark overlay. Space returns to the same match.
/// Strikes are ignored, and the match clock does not move.
/// </summary>
public sealed class pause_State : i_Game_State
{
    private const float overlay_Opacity = 0.53f;
    private const float heading_Y = 300f;
    private const float prompt_Y = 380f;

    private readonly game_Services services;
    private readonly playing_State paused_Match;

    /// <summary>
    /// Pauses the given match.
    /// </summary>
    public pause_State(game_Services services, playing_State paused_Match)
    {
        this.services = services;
        this.paused_Match = paused_Match;
    }

    /// <summary>
    /// Resumes the match on Space; every other input is ignored.
    /// </summary>
    public void update(double real_Delta_Seconds, input_Snapshot input)
    {
        if (input.pause_Pressed)
        {
            services.state_Machine.change_State(paused_Match);
        }
    }

    /// <summary>
    /// Draws the paused match, a dark overlay, and the pause texts.
    /// </summary>
    public void draw(render_Context ctx)
    {
        paused_Match.draw(ctx);
        ctx.fill_Screen(Color.Black * overlay_Opacity, world_Layout.screen_Width, world_Layout.screen_Height);

        var center_X = world_Layout.screen_Width / 2f;
        ctx.draw_Centered_Text(ctx.title_Font, ui_Texts.pause_Title, new Vector2(center_X, heading_Y), game_Colors.text);
        ctx.draw_Centered_Text(ctx.hud_Font, ui_Texts.resume_Prompt, new Vector2(center_X, prompt_Y), game_Colors.text);
    }
}
