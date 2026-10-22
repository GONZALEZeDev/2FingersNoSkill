using two_Fingers_No_Skill.config;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// Picks the active difficulty tier from the elapsed match time.
/// </summary>
public sealed class difficulty_Manager
{
    private readonly IReadOnlyList<difficulty_Tier> tiers;

    /// <summary>
    /// Creates the manager.
    /// </summary>
    /// <param name="tiers">Tiers sorted by start time, the first one starting at 0 (guaranteed by settings_Loader).</param>
    public difficulty_Manager(IReadOnlyList<difficulty_Tier> tiers)
    {
        this.tiers = tiers;
    }

    /// <summary>
    /// Returns the index of the last tier whose start time has been reached.
    /// </summary>
    public int get_Tier_Index_At(double elapsed_Game_Seconds)
    {
        var active_Index = 0;
        for (var index = 1; index < tiers.Count; index++)
        {
            if (tiers[index].start_Time_Seconds > elapsed_Game_Seconds)
            {
                break;
            }
            active_Index = index;
        }
        return active_Index;
    }

    /// <summary>
    /// Returns the tier active at the given match time.
    /// </summary>
    public difficulty_Tier get_Tier_At(double elapsed_Game_Seconds) => tiers[get_Tier_Index_At(elapsed_Game_Seconds)];

    /// <summary>
    /// Returns the rhythm shown to the player: enemy walking speed of the active tier
    /// relative to the first tier, as a rounded percentage (first tier = 100).
    /// </summary>
    public int get_Rhythm_Percent_At(double elapsed_Game_Seconds) =>
        (int)Math.Round(get_Tier_At(elapsed_Game_Seconds).enemy_Speed / tiers[0].enemy_Speed * 100, MidpointRounding.AwayFromZero);
}
