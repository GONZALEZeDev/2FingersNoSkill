namespace two_Fingers_No_Skill.config;

/// <summary>
/// Thrown when game_Settings.json is readable but contains values the game cannot use.
/// </summary>
public sealed class settings_Validation_Exception : Exception
{
    /// <summary>
    /// Creates the exception with a human-readable reason.
    /// </summary>
    public settings_Validation_Exception(string message) : base(message)
    {
    }
}
