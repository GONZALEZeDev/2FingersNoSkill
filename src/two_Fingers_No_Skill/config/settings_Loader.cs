using System.Text.Json;

namespace two_Fingers_No_Skill.config;

/// <summary>
/// Reads and validates game_Settings.json, falling back to the defaults when the file is unusable.
/// </summary>
public static class settings_Loader
{
    /// <summary>Name of the settings file, expected next to the executable.</summary>
    public const string default_File_Name = "game_Settings.json";

    private static readonly JsonSerializerOptions json_Options = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        // A misspelled key (e.g. wrong case) is an error instead of being silently ignored.
        UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow,
    };

    /// <summary>
    /// Loads the settings file. Never throws: on any problem, reports it and returns the defaults.
    /// </summary>
    /// <param name="file_Path">Path of the JSON file.</param>
    /// <param name="report_Error">Optional callback receiving an error message.</param>
    public static game_Settings load_From_File(string file_Path, Action<string>? report_Error = null)
    {
        try
        {
            return parse_Json(File.ReadAllText(file_Path));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException or settings_Validation_Exception)
        {
            report_Error?.Invoke($"[settings] Could not use '{file_Path}': {exception.Message} Default settings are used instead.");
            return game_Settings.create_Default();
        }
    }

    /// <summary>
    /// Parses and validates settings JSON. Tiers are sorted by start time.
    /// </summary>
    /// <exception cref="JsonException">The text is not valid JSON.</exception>
    /// <exception cref="settings_Validation_Exception">A value is out of range.</exception>
    public static game_Settings parse_Json(string json)
    {
        var settings = JsonSerializer.Deserialize<game_Settings>(json, json_Options)
            ?? throw new settings_Validation_Exception("The file is empty.");

        require_No_Null_Section(settings);
        // Sorting lets the author list tiers in any order; the difficulty manager relies on ascending order.
        settings.difficulty_Tiers = settings.difficulty_Tiers.OrderBy(tier => tier.start_Time_Seconds).ToList();
        validate(settings);
        return settings;
    }

    /// <summary>
    /// JSON "null" is accepted by the deserializer even for required sections, so they are checked here.
    /// </summary>
    private static void require_No_Null_Section(game_Settings settings)
    {
        require(settings.score_Rules is not null, "score_Rules must not be null.");
        require(settings.player is not null, "player must not be null.");
        require(settings.event_Log is not null, "event_Log must not be null.");
        require(settings.difficulty_Tiers is not null, "difficulty_Tiers must not be null.");
        require(settings.difficulty_Tiers!.All(tier => tier is not null), "difficulty_Tiers must not contain null.");
    }

    /// <summary>
    /// Throws a settings_Validation_Exception describing the first invalid value found.
    /// </summary>
    private static void validate(game_Settings settings)
    {
        require(settings.starting_Lives >= 1, "starting_Lives must be at least 1.");
        require(settings.minimum_Score >= 0, "minimum_Score must not be negative.");
        require(settings.slow_Motion_Time_Scale > 0 && settings.slow_Motion_Time_Scale <= 1, "slow_Motion_Time_Scale must be in ]0, 1].");
        require(settings.player.attack_Range > 0, "player.attack_Range must be positive.");
        require(settings.player.contact_Distance >= 0, "player.contact_Distance must not be negative.");
        require(settings.player.miss_Lockout_Seconds >= 0, "player.miss_Lockout_Seconds must not be negative.");
        require(settings.event_Log.max_Visible_Events >= 1, "event_Log.max_Visible_Events must be at least 1.");
        require(settings.difficulty_Tiers.Count > 0, "difficulty_Tiers must contain at least one tier.");
        require(settings.difficulty_Tiers[0].start_Time_Seconds == 0, "The first difficulty tier must start at 0.");

        foreach (var tier in settings.difficulty_Tiers)
        {
            var label = $"Tier starting at {tier.start_Time_Seconds}s:";
            require(tier.spawn_Interval_Seconds > 0, $"{label} spawn_Interval_Seconds must be positive.");
            require(tier.enemy_Speed > 0, $"{label} enemy_Speed must be positive.");
            require(tier.max_Enemies_On_Screen >= 1, $"{label} max_Enemies_On_Screen must be at least 1.");
            require(tier.slow_Motion_Duration_Seconds >= 0, $"{label} slow_Motion_Duration_Seconds must not be negative.");
        }
    }

    /// <summary>
    /// Throws with the given message when the condition is false.
    /// </summary>
    private static void require(bool condition, string message)
    {
        if (!condition)
        {
            throw new settings_Validation_Exception(message);
        }
    }
}
