using System.Globalization;
using Microsoft.Xna.Framework;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.game_States;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.rendering;

/// <summary>
/// Draws the match HUD: score (top center), lives (bottom right) and event log (bottom left).
/// Each part slides in from its own edge while slide_Progress goes from 0 to 1.
/// </summary>
public static class hud_Renderer
{
    private const float score_Y = 40f;
    private const float screen_Margin = 24f;
    private const float life_Size = 22f;
    private const float life_Spacing = 8f;
    private const float log_Bottom_Y = 690f;
    private const float log_Line_Height = 24f;
    private const float log_Opacity_Step = 0.15f;
    private const float log_Min_Opacity = 0.25f;
    private const float slide_Distance = 400f;

    /// <summary>
    /// Draws the score centered at the top; slides down from above the screen.
    /// </summary>
    public static void draw_Score(render_Context ctx, int score, float slide_Progress)
    {
        var y = MathHelper.Lerp(-score_Y, score_Y, slide_Progress);
        var text = string.Format(CultureInfo.InvariantCulture, ui_Texts.score_Format, score);
        ctx.draw_Centered_Text(ctx.hud_Font, text, new Vector2(world_Layout.screen_Width / 2f, y), game_Colors.text);
    }

    /// <summary>
    /// Draws one red square per life, right-aligned; slides in from the right edge.
    /// </summary>
    public static void draw_Lives(render_Context ctx, int lives, float slide_Progress)
    {
        var offset_X = (1f - slide_Progress) * slide_Distance;
        var y = world_Layout.screen_Height - screen_Margin - life_Size;
        for (var index = 0; index < lives; index++)
        {
            // Index 0 is the rightmost square.
            var x = world_Layout.screen_Width - screen_Margin - life_Size - index * (life_Size + life_Spacing) + offset_X;
            ctx.shapes.fill_Rectangle(ctx.batch, new Vector2(x, y), new Vector2(life_Size, life_Size), game_Colors.enemy);
        }
    }

    /// <summary>
    /// Draws the event log, newest line at the bottom, older lines above and fading; slides in from the left edge.
    /// </summary>
    /// <param name="entries">Entries ordered oldest first, newest last.</param>
    public static void draw_Event_Log(render_Context ctx, IReadOnlyList<event_Log_Entry> entries, float slide_Progress)
    {
        var x = screen_Margin - (1f - slide_Progress) * slide_Distance;
        for (var age = 0; age < entries.Count; age++)
        {
            var entry = entries[entries.Count - 1 - age];
            var opacity = Math.Max(log_Min_Opacity, 1f - age * log_Opacity_Step);
            var y = log_Bottom_Y - log_Line_Height * (age + 1);
            ctx.batch.DrawString(ctx.hud_Font, entry.text, new Vector2(MathF.Round(x), y), game_Colors.text * opacity);
        }
    }

    /// <summary>
    /// Draws the three HUD parts of a match.
    /// </summary>
    public static void draw_All(render_Context ctx, match_World world, float slide_Progress)
    {
        draw_Score(ctx, world.score.current_Score, slide_Progress);
        draw_Lives(ctx, world.current_Lives, slide_Progress);
        draw_Event_Log(ctx, world.event_Log.entries, slide_Progress);
    }
}
