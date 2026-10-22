using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.entities;
using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// A match in progress: forwards strikes to the match world, draws it and the HUD, and ends on game over.
/// </summary>
public sealed class playing_State : i_Game_State
{
    /// <summary>Duration of the HUD slide-in animation at match start.</summary>
    public const double hud_Slide_Duration_Seconds = 0.4;

    private readonly game_Services services;
    private double slide_Timer_Seconds;

    /// <summary>
    /// Starts a fresh match.
    /// </summary>
    public playing_State(game_Services services)
    {
        this.services = services;
        world = new match_World(services.settings, services.random);
    }

    /// <summary>The match being played.</summary>
    public match_World world { get; }

    /// <summary>
    /// Applies strikes (left first, then right), advances the match, and switches to game over when needed.
    /// </summary>
    public void update(double real_Delta_Seconds, input_Snapshot input)
    {
        slide_Timer_Seconds += real_Delta_Seconds;

        if (input.left_Strike_Pressed)
        {
            world.handle_Strike(facing_Direction.left);
        }
        if (input.right_Strike_Pressed)
        {
            world.handle_Strike(facing_Direction.right);
        }

        world.update(real_Delta_Seconds);

        if (world.is_Game_Over)
        {
            services.state_Machine.change_State(new game_Over_State(services, world));
        }
    }

    /// <summary>
    /// Draws the play area, the optional debug zones and the sliding HUD.
    /// </summary>
    public void draw(render_Context ctx)
    {
        world_Renderer.draw_Ground(ctx);
        world_Renderer.draw_Enemies(ctx, world.enemies);
        world_Renderer.draw_Player(ctx, world.player);
        if (services.is_Debug_View_Enabled)
        {
            world_Renderer.draw_Debug_Zones(ctx, world.player, services.settings.player.attack_Range);
        }

        var slide_Progress = easing.ease_Out_Cubic((float)(slide_Timer_Seconds / hud_Slide_Duration_Seconds));
        hud_Renderer.draw_All(ctx, world, slide_Progress);
    }
}
