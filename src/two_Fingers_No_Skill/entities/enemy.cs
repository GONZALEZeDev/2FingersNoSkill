namespace two_Fingers_No_Skill.entities;

/// <summary>
/// An enemy walking in a straight line toward the player.
/// </summary>
public sealed class enemy
{
    /// <summary>
    /// Creates an enemy.
    /// </summary>
    /// <param name="spawn_Side">Side of the screen it comes from.</param>
    /// <param name="center_X">Initial horizontal center, in pixels.</param>
    /// <param name="half_Width">Half of the body width, in pixels.</param>
    /// <param name="speed">Walking speed, in pixels per second.</param>
    public enemy(facing_Direction spawn_Side, float center_X, float half_Width, float speed)
    {
        this.spawn_Side = spawn_Side;
        this.center_X = center_X;
        this.half_Width = half_Width;
        this.speed = speed;
    }

    /// <summary>Side of the screen the enemy comes from.</summary>
    public facing_Direction spawn_Side { get; }

    /// <summary>Current horizontal center, in pixels.</summary>
    public float center_X { get; private set; }

    /// <summary>Half of the body width, in pixels.</summary>
    public float half_Width { get; }

    /// <summary>Walking speed, in pixels per second.</summary>
    public float speed { get; }

    /// <summary>
    /// Distance between the enemy edge and the player edge that face each other. Never negative.
    /// </summary>
    public float get_Gap_To(player target)
    {
        var gap = spawn_Side == facing_Direction.left
            ? (target.center_X - target.half_Width) - (center_X + half_Width)
            : (center_X - half_Width) - (target.center_X + target.half_Width);
        return Math.Max(0, gap);
    }

    /// <summary>
    /// Walks toward the player, stopping at contact instead of overlapping it.
    /// </summary>
    public void move_Toward(player target, double game_Delta_Seconds)
    {
        // Clamping the step to the remaining gap also covers huge frame deltas.
        var step = (float)Math.Min(speed * game_Delta_Seconds, get_Gap_To(target));
        center_X += spawn_Side == facing_Direction.left ? step : -step;
    }
}
