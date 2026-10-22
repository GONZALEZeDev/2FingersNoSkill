namespace two_Fingers_No_Skill.entities;

/// <summary>
/// The character controlled by the player. It never moves; it only turns and strikes.
/// </summary>
public sealed class player
{
    private double lockout_Remaining_Seconds;

    /// <summary>
    /// Creates the player, facing left.
    /// </summary>
    /// <param name="center_X">Horizontal center, in pixels.</param>
    /// <param name="half_Width">Half of the body width, in pixels.</param>
    public player(float center_X, float half_Width)
    {
        this.center_X = center_X;
        this.half_Width = half_Width;
    }

    /// <summary>Horizontal center, in pixels.</summary>
    public float center_X { get; }

    /// <summary>Half of the body width, in pixels.</summary>
    public float half_Width { get; }

    /// <summary>Side the player is looking at (the side of the last strike).</summary>
    public facing_Direction facing { get; private set; } = facing_Direction.left;

    /// <summary>True while strikes are blocked after a missed hit.</summary>
    public bool is_Locked_Out => lockout_Remaining_Seconds > 0;

    /// <summary>
    /// Turns the player toward a side.
    /// </summary>
    public void face(facing_Direction direction)
    {
        facing = direction;
    }

    /// <summary>
    /// Blocks strikes for the given duration (penalty of a missed hit).
    /// </summary>
    public void start_Lockout(double duration_Seconds)
    {
        lockout_Remaining_Seconds = duration_Seconds;
    }

    /// <summary>
    /// Advances the player timers by one frame.
    /// </summary>
    public void update(double game_Delta_Seconds)
    {
        // A small epsilon avoids floating point leftovers keeping the lockout alive one extra frame.
        lockout_Remaining_Seconds = Math.Max(0, lockout_Remaining_Seconds - game_Delta_Seconds);
        if (lockout_Remaining_Seconds < 1e-9)
        {
            lockout_Remaining_Seconds = 0;
        }
    }
}
