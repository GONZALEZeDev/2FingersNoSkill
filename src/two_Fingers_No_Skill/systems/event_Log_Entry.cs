using two_Fingers_No_Skill.core;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// One line of the on-screen event log.
/// </summary>
/// <param name="event_Type">The event the line describes.</param>
/// <param name="text">The French text displayed.</param>
public readonly record struct event_Log_Entry(game_Event_Type event_Type, string text);
