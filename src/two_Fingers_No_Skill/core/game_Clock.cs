namespace two_Fingers_No_Skill.core;

/// <summary>
/// Converts real time into match time and handles the "almost paused" slow motion after a hit.
/// </summary>
public sealed class game_Clock
{
    private readonly float slow_Motion_Time_Scale;
    private double slow_Motion_Remaining_Real_Seconds;

    /// <summary>
    /// Creates a clock at match time 0.
    /// </summary>
    /// <param name="slow_Motion_Time_Scale">Speed multiplier applied while slow motion is active.</param>
    public game_Clock(float slow_Motion_Time_Scale)
    {
        this.slow_Motion_Time_Scale = slow_Motion_Time_Scale;
    }

    /// <summary>Match time elapsed so far (slowed time included), used for difficulty tiers.</summary>
    public double elapsed_Game_Seconds { get; private set; }

    /// <summary>True while the slow motion is running.</summary>
    public bool is_Slow_Motion_Active => slow_Motion_Remaining_Real_Seconds > 0;

    /// <summary>
    /// Starts (or restarts) slow motion for the given real-time duration.
    /// </summary>
    public void start_Slow_Motion(double duration_Real_Seconds)
    {
        slow_Motion_Remaining_Real_Seconds = duration_Real_Seconds;
    }

    /// <summary>
    /// Advances the clock by one frame and returns the match-time delta to use for gameplay.
    /// </summary>
    /// <param name="real_Delta_Seconds">Real time elapsed since the previous frame.</param>
    public double advance(double real_Delta_Seconds)
    {
        var game_Delta = real_Delta_Seconds;
        if (is_Slow_Motion_Active)
        {
            // The slow motion lasts a fixed real duration, so it is counted down with the real delta.
            game_Delta *= slow_Motion_Time_Scale;
            slow_Motion_Remaining_Real_Seconds -= real_Delta_Seconds;
        }

        elapsed_Game_Seconds += game_Delta;
        return game_Delta;
    }
}
