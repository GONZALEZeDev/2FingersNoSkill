using System.Globalization;
using Microsoft.Xna.Framework;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.game_States;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.rendering;

/// <summary>
/// Draws the match HUD: score (top center), rhythm (top left), timer (top right), lives (bottom right)
/// and event log (bottom left). Each part slides in from its own edge while slide_Progress goes from 0 to 1.
/// </summary>
public static class hud_Renderer
{
    private const float score_Y = 40f;
    private const float screen_Margin = 24f;
    private const float life_Size = 22f;
    private const float life_Spacing = 8f;
    private const float log_Bottom_Y = 690f;
    // Negative: the Consolas line spacing includes extra leading, so lines are pulled closer together
    // to keep a full log below the ground line.
    private const float log_Line_Spacing = -4f;
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
    /// Draws the match timer ("MM:SS") right-aligned in the top right corner; slides in from the right edge.
    /// </summary>
    public static void draw_Timer(render_Context ctx, double elapsed_Game_Seconds, float slide_Progress)
    {
        var text = time_Formatter.format_Duration(elapsed_Game_Seconds);
        var width = ctx.hud_Font.MeasureString(text).X;
        var x = world_Layout.screen_Width - screen_Margin - width + (1f - slide_Progress) * slide_Distance;
        ctx.batch.DrawString(ctx.hud_Font, text, new Vector2(MathF.Round(x), screen_Margin), game_Colors.text);
    }

    /// <summary>
    /// Draws the current rhythm percent in the top left corner; slides in from the left edge.
    /// </summary>
    public static void draw_Rhythm(render_Context ctx, int rhythm_Percent, float slide_Progress)
    {
        var x = screen_Margin - (1f - slide_Progress) * slide_Distance;
        var text = string.Format(CultureInfo.InvariantCulture, ui_Texts.rhythm_Format, rhythm_Percent);
        ctx.batch.DrawString(ctx.hud_Font, text, new Vector2(MathF.Round(x), screen_Margin), game_Colors.highlight);
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
    /// Rhythm increases stand out in gold with a larger font.
    /// </summary>
    /// <param name="entries">Entries ordered oldest first, newest last.</param>
    public static void draw_Event_Log(render_Context ctx, IReadOnlyList<event_Log_Entry> entries, float slide_Progress)
    {
        var x = MathF.Round(screen_Margin - (1f - slide_Progress) * slide_Distance);
        var line_Bottom = log_Bottom_Y;
        for (var age = 0; age < entries.Count; age++)
        {
            var entry = entries[entries.Count - 1 - age];
            var is_Highlighted = entry.event_Type == game_Event_Type.rhythm_Increased;
            var font = is_Highlighted ? ctx.highlight_Font : ctx.hud_Font;
            var color = is_Highlighted ? game_Colors.highlight : game_Colors.text;
            var opacity = Math.Max(log_Min_Opacity, 1f - age * log_Opacity_Step);

            // Lines are stacked upward by their own height, since highlighted lines are taller.
            line_Bottom -= font.LineSpacing;
            ctx.batch.DrawString(font, entry.text, new Vector2(x, line_Bottom), color * opacity);
            line_Bottom -= log_Line_Spacing;
        }
    }

    /// <summary>
    /// Draws every HUD part of a match.
    /// </summary>
    public static void draw_All(render_Context ctx, match_World world, float slide_Progress)
    {
        draw_Score(ctx, world.score.current_Score, slide_Progress);
        draw_Timer(ctx, world.clock.elapsed_Game_Seconds, slide_Progress);
        draw_Rhythm(ctx, world.current_Rhythm_Percent, slide_Progress);
        draw_Lives(ctx, world.current_Lives, slide_Progress);
        draw_Event_Log(ctx, world.event_Log.entries, slide_Progress);
    }
}
