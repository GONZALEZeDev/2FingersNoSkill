using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.core;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// Keeps the latest gameplay events as French text lines for the on-screen log.
/// </summary>
public sealed class event_Log
{
    private const string enemy_Killed_Label = "Ennemi tué";
    private const string life_Lost_Label = "Vie perdue";
    private const string missed_Hit_Label = "Coup raté";

    private readonly int max_Entries;
    private readonly score_Rules rules;
    private readonly List<event_Log_Entry> entry_List = new();

    /// <summary>
    /// Creates an empty log.
    /// </summary>
    /// <param name="max_Entries">Number of entries kept; older ones are dropped.</param>
    /// <param name="rules">Score rules, used to show the points of each event.</param>
    public event_Log(int max_Entries, score_Rules rules)
    {
        this.max_Entries = max_Entries;
        this.rules = rules;
    }

    /// <summary>Entries ordered oldest first, newest last.</summary>
    public IReadOnlyList<event_Log_Entry> entries => entry_List;

    /// <summary>
    /// Adds a line for the event and drops the oldest one beyond the limit. Subscribed to the game_Event_Bus.
    /// </summary>
    public void handle_Event(game_Event game_Event)
    {
        // Same mapping as the score manager, so the log and the score always agree on the points.
        var points = score_Manager.get_Points_For(rules, game_Event.event_Type);
        entry_List.Add(new event_Log_Entry(game_Event.event_Type, build_Text(game_Event.event_Type, points)));
        if (entry_List.Count > max_Entries)
        {
            entry_List.RemoveAt(0);
        }
    }

    /// <summary>
    /// Builds the French line for an event, e.g. "Ennemi tué (+20)". No suffix when points are 0.
    /// </summary>
    public static string build_Text(game_Event_Type event_Type, int points)
    {
        var label = event_Type switch
        {
            game_Event_Type.enemy_Killed => enemy_Killed_Label,
            game_Event_Type.life_Lost => life_Lost_Label,
            game_Event_Type.missed_Hit => missed_Hit_Label,
            _ => event_Type.ToString(),
        };

        if (points == 0)
        {
            return label;
        }

        // ASCII "+"/"-" only: the SpriteFont covers the 32-255 character range.
        var sign = points > 0 ? "+" : "-";
        return $"{label} ({sign}{Math.Abs(points)})";
    }
}
