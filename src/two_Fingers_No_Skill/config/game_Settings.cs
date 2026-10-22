using System.Text.Json.Serialization;

namespace two_Fingers_No_Skill.config;

/// <summary>
/// All tunable gameplay values. Property names match the keys of game_Settings.json exactly.
/// Every key is required in the JSON, so a missing or misspelled key is reported instead of silently becoming 0.
/// </summary>
public sealed class game_Settings
{
    /// <summary>Lives at the start of each match.</summary>
    [JsonRequired]
    public int starting_Lives { get; set; }

    /// <summary>Points for each kind of game event.</summary>
    [JsonRequired]
    public score_Rules score_Rules { get; set; } = new();

    /// <summary>The score never goes below this value (0 or more).</summary>
    [JsonRequired]
    public int minimum_Score { get; set; }

    /// <summary>Combat values related to the player.</summary>
    [JsonRequired]
    public player_Settings player { get; set; } = new();

    /// <summary>Game speed multiplier while slow motion is active (0.05 = 5% speed).</summary>
    [JsonRequired]
    public float slow_Motion_Time_Scale { get; set; }

    /// <summary>Event log display settings.</summary>
    [JsonRequired]
    public event_Log_Settings event_Log { get; set; } = new();

    /// <summary>Difficulty tiers, sorted by start time.</summary>
    [JsonRequired]
    public List<difficulty_Tier> difficulty_Tiers { get; set; } = new();

    /// <summary>
    /// Builds the default settings. They must stay identical to the shipped game_Settings.json.
    /// </summary>
    public static game_Settings create_Default() => new()
    {
        starting_Lives = 3,
        score_Rules = new score_Rules { enemy_Killed = 20, missed_Hit = -50, life_Lost = 0 },
        minimum_Score = 0,
        player = new player_Settings { attack_Range = 140, contact_Distance = 30, miss_Lockout_Seconds = 0.3 },
        slow_Motion_Time_Scale = 0.05f,
        event_Log = new event_Log_Settings { max_Visible_Events = 6 },
        difficulty_Tiers = new List<difficulty_Tier>
        {
            create_Tier(0, 1.6, 140, 3, 1.25),
            create_Tier(90, 1.4, 155, 3, 1.25),
            create_Tier(180, 1.2, 170, 4, 1.25),
            create_Tier(270, 1.05, 185, 4, 1.15),
            create_Tier(360, 0.9, 200, 5, 1.05),
            create_Tier(450, 0.65, 240, 6, 0.95),
            create_Tier(540, 0.45, 290, 8, 0.85),
        },
    };

    /// <summary>
    /// Shorthand used by create_Default to keep the tier table readable.
    /// </summary>
    private static difficulty_Tier create_Tier(double start_Time, double spawn_Interval, float speed, int max_Enemies, double slow_Motion) => new()
    {
        start_Time_Seconds = start_Time,
        spawn_Interval_Seconds = spawn_Interval,
        enemy_Speed = speed,
        max_Enemies_On_Screen = max_Enemies,
        slow_Motion_Duration_Seconds = slow_Motion,
    };
}
