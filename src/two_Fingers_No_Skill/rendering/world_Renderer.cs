using Microsoft.Xna.Framework;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.entities;

namespace two_Fingers_No_Skill.rendering;

/// <summary>
/// Draws the play area: ground, player, enemies and the debug strike zones.
/// </summary>
public static class world_Renderer
{
    private const float ground_Thickness = 2f;
    private const float arm_Length = 26f;
    private const float arm_Height = 10f;
    private const float arm_Top_Offset = 40f;

    /// <summary>
    /// Draws the ground line across the screen.
    /// </summary>
    public static void draw_Ground(render_Context ctx)
    {
        ctx.shapes.fill_Rectangle(ctx.batch, new Vector2(0, world_Layout.ground_Y), new Vector2(world_Layout.screen_Width, ground_Thickness), game_Colors.ground);
    }

    /// <summary>
    /// Draws the player body and an "arm" on the side it faces. Grey while locked out after a miss.
    /// </summary>
    public static void draw_Player(render_Context ctx, player target)
    {
        var color = target.is_Locked_Out ? game_Colors.locked_Out_Player : game_Colors.player;
        var body_Left = target.center_X - target.half_Width;
        var body_Top = world_Layout.ground_Y - world_Layout.player_Height;
        ctx.shapes.fill_Rectangle(ctx.batch, new Vector2(body_Left, body_Top), new Vector2(target.half_Width * 2, world_Layout.player_Height), color);

        var arm_X = target.facing == facing_Direction.left
            ? body_Left - arm_Length
            : target.center_X + target.half_Width;
        ctx.shapes.fill_Rectangle(ctx.batch, new Vector2(arm_X, body_Top + arm_Top_Offset), new Vector2(arm_Length, arm_Height), color);
    }

    /// <summary>
    /// Draws every enemy body.
    /// </summary>
    public static void draw_Enemies(render_Context ctx, IReadOnlyList<enemy> enemies)
    {
        foreach (var walking_Enemy in enemies)
        {
            var top_Left = new Vector2(walking_Enemy.center_X - walking_Enemy.half_Width, world_Layout.ground_Y - world_Layout.enemy_Height);
            ctx.shapes.fill_Rectangle(ctx.batch, top_Left, new Vector2(walking_Enemy.half_Width * 2, world_Layout.enemy_Height), game_Colors.enemy);
        }
    }

    /// <summary>
    /// Draws both strike zones as dashed rectangles (debug view, toggled with F1).
    /// </summary>
    public static void draw_Debug_Zones(render_Context ctx, player target, float attack_Range)
    {
        var zone_Top = world_Layout.ground_Y - world_Layout.player_Height - 20f;
        var zone_Size = new Vector2(attack_Range, world_Layout.player_Height + 20f);
        ctx.shapes.draw_Dashed_Outline(ctx.batch, new Vector2(target.center_X - target.half_Width - attack_Range, zone_Top), zone_Size, game_Colors.debug_Zone);
        ctx.shapes.draw_Dashed_Outline(ctx.batch, new Vector2(target.center_X + target.half_Width, zone_Top), zone_Size, game_Colors.debug_Zone);
    }
}
