using two_Fingers_No_Skill.config;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// Objects shared by every screen, handed to each state at creation.
/// </summary>
public sealed class game_Services
{
    /// <summary>
    /// Groups the shared objects.
    /// </summary>
    public game_Services(game_Settings settings, game_State_Machine state_Machine, Random random)
    {
        this.settings = settings;
        this.state_Machine = state_Machine;
        this.random = random;
    }

    /// <summary>Gameplay settings loaded at startup.</summary>
    public game_Settings settings { get; }

    /// <summary>Used by screens to switch to another screen.</summary>
    public game_State_Machine state_Machine { get; }

    /// <summary>Random source shared by matches.</summary>
    public Random random { get; }

    /// <summary>True when the debug view (strike zones) is shown. Toggled with F1.</summary>
    public bool is_Debug_View_Enabled { get; set; }
}
