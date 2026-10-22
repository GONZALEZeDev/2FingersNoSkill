using System.Globalization;
using Microsoft.Xna.Framework;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// Game over screen: the match is frozen, the player is gone, and the final stats are shown. Enter restarts.
/// </summary>
public sealed class game_Over_State : i_Game_State
{
    private const float overlay_Opacity = 0.53f;
    private const float heading_Y = 220f;
    private const float first_Stat_Y = 320f;
    private const float stat_Line_Height = 30f;
    private const float prompt_Y = 580f;

    private readonly game_Services services;
    private readonly match_World finished_World;

    /// <summary>
    /// Creates the screen for a finished match.
    /// </summary>
    public game_Over_State(game_Services services, match_World finished_World)
    {
        this.services = services;
        this.finished_World = finished_World;
    }

    /// <summary>
    /// The match is not updated anymore (frozen). Enter starts the restart flash.
    /// </summary>
    public void update(double real_Delta_Seconds, input_Snapshot input)
    {
        if (input.confirm_Pressed)
        {
            services.state_Machine.change_State(new restart_Flash_State(services, this));
        }
    }

    /// <summary>
    /// Draws the frozen enemies (no player), the HUD, a dark overlay and the final stats.
    /// </summary>
    public void draw(render_Context ctx)
    {
        world_Renderer.draw_Ground(ctx);
        world_Renderer.draw_Enemies(ctx, finished_World.enemies);
        hud_Renderer.draw_All(ctx, finished_World, 1f);
        ctx.fill_Screen(Color.Black * overlay_Opacity, world_Layout.screen_Width, world_Layout.screen_Height);

        var center_X = world_Layout.screen_Width / 2f;
        ctx.draw_Centered_Text(ctx.title_Font, ui_Texts.game_Over_Title, new Vector2(center_X, heading_Y), game_Colors.enemy);

        var stat_Lines = new[]
        {
            format(ui_Texts.final_Score_Format, finished_World.score.current_Score),
            format(ui_Texts.enemies_Killed_Format, finished_World.stats.enemies_Killed),
            format(ui_Texts.missed_Hits_Format, finished_World.stats.missed_Hits),
        };
        for (var index = 0; index < stat_Lines.Length; index++)
        {
            ctx.draw_Centered_Text(ctx.hud_Font, stat_Lines[index], new Vector2(center_X, first_Stat_Y + index * stat_Line_Height), game_Colors.text);
        }

        ctx.draw_Centered_Text(ctx.hud_Font, ui_Texts.restart_Prompt, new Vector2(center_X, prompt_Y), game_Colors.text);
    }

    /// <summary>
    /// Fills a text template with a number.
    /// </summary>
    private static string format(string template, int value) => string.Format(CultureInfo.InvariantCulture, template, value);
}
