namespace two_Fingers_No_Skill.core;

/// <summary>
/// Every kind of gameplay event. Adding one means: a value here, a rule in score_Rules, a label in event_Log.
/// </summary>
public enum game_Event_Type
{
    /// <summary>The player killed an enemy.</summary>
    enemy_Killed,

    /// <summary>An enemy reached and hit the player.</summary>
    life_Lost,

    /// <summary>The player struck a side with no enemy in range.</summary>
    missed_Hit,

    /// <summary>A new difficulty tier started; the event value holds the new rhythm percent.</summary>
    rhythm_Increased,
}
