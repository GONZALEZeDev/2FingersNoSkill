namespace two_Fingers_No_Skill.core;

/// <summary>
/// Broadcasts gameplay events so that systems (score, stats, log...) react without knowing each other.
/// </summary>
public sealed class game_Event_Bus
{
    /// <summary>Raised for every published event.</summary>
    public event Action<game_Event>? event_Published;

    /// <summary>
    /// Sends the event to every subscriber.
    /// </summary>
    public void publish(game_Event game_Event) => event_Published?.Invoke(game_Event);
}
