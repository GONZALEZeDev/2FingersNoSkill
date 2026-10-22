using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace two_Fingers_No_Skill.rendering;

/// <summary>
/// Draws plain rectangles by stretching a single white pixel. All game visuals are built from it.
/// </summary>
public sealed class shape_Renderer
{
    private const float dash_Length = 8f;
    private const float line_Thickness = 2f;

    private readonly Texture2D pixel_Texture;

    /// <summary>
    /// Creates the 1x1 white texture.
    /// </summary>
    public shape_Renderer(GraphicsDevice device)
    {
        pixel_Texture = new Texture2D(device, 1, 1);
        pixel_Texture.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Draws a filled rectangle.
    /// </summary>
    public void fill_Rectangle(SpriteBatch batch, Vector2 top_Left, Vector2 size, Color color)
    {
        batch.Draw(pixel_Texture, top_Left, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);
    }

    /// <summary>
    /// Draws the outline of a rectangle with dashed lines.
    /// </summary>
    public void draw_Dashed_Outline(SpriteBatch batch, Vector2 top_Left, Vector2 size, Color color)
    {
        var bottom_Y = top_Left.Y + size.Y - line_Thickness;
        var right_X = top_Left.X + size.X - line_Thickness;

        for (var x = top_Left.X; x < top_Left.X + size.X; x += dash_Length * 2)
        {
            var length = Math.Min(dash_Length, top_Left.X + size.X - x);
            fill_Rectangle(batch, new Vector2(x, top_Left.Y), new Vector2(length, line_Thickness), color);
            fill_Rectangle(batch, new Vector2(x, bottom_Y), new Vector2(length, line_Thickness), color);
        }

        for (var y = top_Left.Y; y < top_Left.Y + size.Y; y += dash_Length * 2)
        {
            var length = Math.Min(dash_Length, top_Left.Y + size.Y - y);
            fill_Rectangle(batch, new Vector2(top_Left.X, y), new Vector2(line_Thickness, length), color);
            fill_Rectangle(batch, new Vector2(right_X, y), new Vector2(line_Thickness, length), color);
        }
    }
}
