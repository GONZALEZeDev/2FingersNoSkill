using two_Fingers_No_Skill.core;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// Counts what happened during a match, for the game over screen.
/// </summary>
public sealed class match_Stats
{
    /// <summary>Number of enemies killed.</summary>
    public int enemies_Killed { get; private set; }

    /// <summary>Number of strikes that hit nothing.</summary>
    public int missed_Hits { get; private set; }

    /// <summary>Number of lives lost.</summary>
    public int lives_Lost { get; private set; }

    /// <summary>
    /// Increments the counter matching the event. Subscribed to the game_Event_Bus.
    /// </summary>
    public void handle_Event(game_Event game_Event)
    {
        switch (game_Event.event_Type)
        {
            case game_Event_Type.enemy_Killed: enemies_Killed++; break;
            case game_Event_Type.missed_Hit: missed_Hits++; break;
            case game_Event_Type.life_Lost: lives_Lost++; break;
        }
    }
}
