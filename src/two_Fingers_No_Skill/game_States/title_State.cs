using Microsoft.Xna.Framework;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.entities;
using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// Title screen: the player in the middle, the title above, the start prompt below. M1 starts a match.
/// </summary>
public sealed class title_State : i_Game_State
{
    private const float title_Y = 200f;
    private const float prompt_Y = 600f;

    private readonly game_Services services;
    private readonly player idle_Player = new(world_Layout.player_Center_X, world_Layout.player_Half_Width);

    /// <summary>
    /// Creates the title screen.
    /// </summary>
    public title_State(game_Services services)
    {
        this.services = services;
    }

    /// <summary>
    /// Starts a match on M1. That click is consumed here, so it never counts as a strike.
    /// </summary>
    public void update(double real_Delta_Seconds, input_Snapshot input)
    {
        if (input.left_Strike_Pressed)
        {
            services.state_Machine.change_State(new playing_State(services));
        }
    }

    /// <summary>
    /// Draws the ground, the idle player (facing left), the title and the prompt.
    /// </summary>
    public void draw(render_Context ctx)
    {
        world_Renderer.draw_Ground(ctx);
        world_Renderer.draw_Player(ctx, idle_Player);
        ctx.draw_Centered_Text(ctx.title_Font, ui_Texts.game_Title, new Vector2(world_Layout.screen_Width / 2f, title_Y), game_Colors.text);
        ctx.draw_Centered_Text(ctx.hud_Font, ui_Texts.start_Prompt, new Vector2(world_Layout.screen_Width / 2f, prompt_Y), game_Colors.text);
    }
}
