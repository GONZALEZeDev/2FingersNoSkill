using System.Text.Json.Serialization;

namespace two_Fingers_No_Skill.config;

/// <summary>
/// Points awarded (or removed) for each kind of game event. Every key is required in the JSON.
/// </summary>
public sealed class score_Rules
{
    /// <summary>Points for killing an enemy.</summary>
    [JsonRequired]
    public int enemy_Killed { get; set; }

    /// <summary>Points for a strike that hits nothing (usually negative).</summary>
    [JsonRequired]
    public int missed_Hit { get; set; }

    /// <summary>Points for losing a life.</summary>
    [JsonRequired]
    public int life_Lost { get; set; }
}
