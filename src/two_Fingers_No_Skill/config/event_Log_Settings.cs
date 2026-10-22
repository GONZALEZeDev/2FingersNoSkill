using System.Text.Json.Serialization;

namespace two_Fingers_No_Skill.config;

/// <summary>
/// Display settings of the on-screen event log. Every key is required in the JSON.
/// </summary>
public sealed class event_Log_Settings
{
    /// <summary>Number of entries kept and shown at the same time.</summary>
    [JsonRequired]
    public int max_Visible_Events { get; set; }
}
