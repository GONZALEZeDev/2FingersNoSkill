using System.Text.Json.Serialization;

namespace two_Fingers_No_Skill.config;

/// <summary>
/// Combat values related to the player. Every key is required in the JSON.
/// </summary>
public sealed class player_Settings
{
    /// <summary>Width of the strike zone on each side, measured from the player edge, in pixels.</summary>
    [JsonRequired]
    public float attack_Range { get; set; }

    /// <summary>Gap (in pixels) at which an enemy is close enough to hit the player.</summary>
    [JsonRequired]
    public float contact_Distance { get; set; }

    /// <summary>Time during which strikes are ignored after a missed hit, in seconds.</summary>
    [JsonRequired]
    public double miss_Lockout_Seconds { get; set; }
}
