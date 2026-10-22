using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace two_Fingers_No_Skill.rendering;

/// <summary>
/// Everything a screen needs to draw itself, passed to each draw call.
/// </summary>
public sealed class render_Context
{
    /// <summary>
    /// Groups the drawing tools.
    /// </summary>
    public render_Context(SpriteBatch batch, shape_Renderer shapes, SpriteFont hud_Font, SpriteFont title_Font, SpriteFont highlight_Font)
    {
        this.batch = batch;
        this.shapes = shapes;
        this.hud_Font = hud_Font;
        this.title_Font = title_Font;
        this.highlight_Font = highlight_Font;
    }

    /// <summary>Sprite batch, already begun by the caller.</summary>
    public SpriteBatch batch { get; }

    /// <summary>Rectangle drawing helper.</summary>
    public shape_Renderer shapes { get; }

    /// <summary>Small font (HUD, log, prompts).</summary>
    public SpriteFont hud_Font { get; }

    /// <summary>Large font (title, game over).</summary>
    public SpriteFont title_Font { get; }

    /// <summary>Slightly larger bold font for highlighted log lines.</summary>
    public SpriteFont highlight_Font { get; }

    /// <summary>
    /// Draws a filled rectangle covering the whole screen (overlays, flash).
    /// </summary>
    public void fill_Screen(Color color, int screen_Width, int screen_Height) =>
        shapes.fill_Rectangle(batch, Vector2.Zero, new Vector2(screen_Width, screen_Height), color);

    /// <summary>
    /// Draws text centered on a point.
    /// </summary>
    public void draw_Centered_Text(SpriteFont font, string text, Vector2 center, Color color)
    {
        var size = font.MeasureString(text);
        // Rounding keeps glyphs on whole pixels, so they stay sharp.
        var position = new Vector2(MathF.Round(center.X - size.X / 2f), MathF.Round(center.Y - size.Y / 2f));
        batch.DrawString(font, text, position, color);
    }
}
