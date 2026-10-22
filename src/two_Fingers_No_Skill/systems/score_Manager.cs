using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.core;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// Keeps the match score up to date from gameplay events, using the configured score rules.
/// </summary>
public sealed class score_Manager
{
    private readonly score_Rules rules;
    private readonly int minimum_Score;

    /// <summary>
    /// Creates a score at 0 (or at the minimum, if it is higher).
    /// </summary>
    public score_Manager(score_Rules rules, int minimum_Score)
    {
        this.rules = rules;
        this.minimum_Score = minimum_Score;
        current_Score = Math.Max(0, minimum_Score);
    }

    /// <summary>Current score, never below the configured minimum.</summary>
    public int current_Score { get; private set; }

    /// <summary>
    /// Returns the points the rules give for an event type.
    /// </summary>
    public int get_Points_For(game_Event_Type event_Type) => get_Points_For(rules, event_Type);

    /// <summary>
    /// Returns the points the given rules give for an event type. Shared with the event log.
    /// </summary>
    public static int get_Points_For(score_Rules rules, game_Event_Type event_Type) => event_Type switch
    {
        game_Event_Type.enemy_Killed => rules.enemy_Killed,
        game_Event_Type.missed_Hit => rules.missed_Hit,
        game_Event_Type.life_Lost => rules.life_Lost,
        _ => 0,
    };

    /// <summary>
    /// Applies the points of an event to the score. Subscribed to the game_Event_Bus.
    /// </summary>
    public void handle_Event(game_Event game_Event)
    {
        current_Score = Math.Max(minimum_Score, current_Score + get_Points_For(game_Event.event_Type));
    }
}
