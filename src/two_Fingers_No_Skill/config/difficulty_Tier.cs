using System.Text.Json.Serialization;

namespace two_Fingers_No_Skill.config;

/// <summary>
/// One difficulty step of a match. The active tier is the last one whose start time has been reached.
/// Every key is required in the JSON, so a missing or misspelled key is reported instead of silently becoming 0.
/// </summary>
public sealed class difficulty_Tier
{
    /// <summary>Game time (in seconds) at which this tier becomes active.</summary>
    [JsonRequired]
    public double start_Time_Seconds { get; set; }

    /// <summary>Delay between two enemy spawns, in seconds.</summary>
    [JsonRequired]
    public double spawn_Interval_Seconds { get; set; }

    /// <summary>Walking speed of the enemies spawned during this tier, in pixels per second.</summary>
    [JsonRequired]
    public float enemy_Speed { get; set; }

    /// <summary>Maximum number of living enemies at the same time.</summary>
    [JsonRequired]
    public int max_Enemies_On_Screen { get; set; }

    /// <summary>Real-time duration of the slow motion triggered when the player is hit.</summary>
    [JsonRequired]
    public double slow_Motion_Duration_Seconds { get; set; }
}
