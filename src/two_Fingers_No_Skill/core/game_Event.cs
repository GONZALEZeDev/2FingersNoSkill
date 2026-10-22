namespace two_Fingers_No_Skill.core;

/// <summary>
/// Something that happened during a match.
/// </summary>
/// <param name="event_Type">What happened.</param>
/// <param name="game_Time_Seconds">Match time at which it happened.</param>
/// <param name="value">Optional number attached to the event (e.g. the new rhythm percent); 0 when unused.</param>
public readonly record struct game_Event(game_Event_Type event_Type, double game_Time_Seconds, int value = 0);
